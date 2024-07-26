using FluentValidation;
using Lidas.WishlistApi.Bus;
using Lidas.WishlistApi.Config;
using Lidas.WishlistApi.Interfaces;
using Lidas.WishlistApi.Validators;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lidas.WishlistApi;

internal static class InfrastructureModule
{
    public static IServiceCollection AddRabbitMQService(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("MassTransitConnection").Get<MassTransitSettings>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<ConsumerTest>();

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

        return services;
    }

    public static void AddValidatorService(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<WishValidator>();
        services.AddScoped<IValidatorService, ValidatorService>();

    }
}
