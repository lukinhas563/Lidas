using FluentValidation;
using Lidas.UserApi.Config;
using Lidas.UserApi.Interfaces;
using Lidas.UserApi.Services;
using Lidas.UserApi.Validators;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Lidas.UserApi;

internal static class InfrastructureModule
{
    public static void AddValidatorsService(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginValidator>();
        services.AddScoped<IValidatorService, Validator>();
    }

    public static void AddTokenService(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenSettings = configuration.GetSection("JWT");
        services.Configure<TokenSettings>(tokenSettings);

        services.AddScoped<IToken, TokenService>();
    }

    public static void AddCryptographyService(this IServiceCollection services)
    {
        services.AddScoped<ICryptography, CryptographyService>();
    }

    public static void AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("SMTP");
        services.Configure<EmailSettings>(emailSettings);

        services.AddScoped<IEmail, EmailService>();
    }

    public static void AddCorsService(this IServiceCollection services, string corsPolicy)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: corsPolicy, policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });
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

    public static void AddMassTransitService(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("MassTransitConnection").Get<MassTransitSettings>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

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
}
