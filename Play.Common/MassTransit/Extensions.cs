using System;
using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(config =>
        {
            config.AddConsumers(Assembly.GetEntryAssembly());

            config.UsingRabbitMq((ctx, cfg) =>
            {
                // I am not using the settings of the RabbitMqSettings, because aspire injected an environment variable of the correct connection string.
                //var rabbitMqSettings =
                //    builder.Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
                //cfg.Host(rabbitMqSettings);
                
                var configuration = ctx.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var rabbitmqConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__rabbitmq");

                cfg.Host(rabbitmqConnectionString);
                cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                cfg.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                });
            });
        });

        services.AddMassTransitHostedService();

        return services;
    }
}