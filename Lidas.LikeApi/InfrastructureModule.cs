using Lidas.LikeApi.Config;
using Lidas.LikeApi.Consumers;
using Lidas.LikeApi.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Refit;
using System.Text;

namespace Lidas.LikeApi;

internal static class InfrastructureModule
{
    public static void AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("MassTransitConnection").Get<MassTransitSettings>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<UserCreateConsumer>();
            busConfigurator.AddConsumer<MangaCreateConsumer>();

            busConfigurator.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(new Uri($"amqp://{settings.Host}:{settings.Port}"), host =>
                {
                    host.Username(settings.Username);
                    host.Password(settings.Password);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    }

    public static void AddCorsPolicyService(this IServiceCollection services, string corsPolicy)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: corsPolicy, policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });
    }

    public static void AddRequestService(this IServiceCollection services)
    {
        services.AddRefitClient<IRequestService>().ConfigureHttpClient(c => c.BaseAddress = new Uri("http://mangaapi:8080"));
    }

    public static void AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        var token = configuration.GetSection("JWT").Get<TokenSettings>();
        var key = Encoding.ASCII.GetBytes(token.Key);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
    }
}
