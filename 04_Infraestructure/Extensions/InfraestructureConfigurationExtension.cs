using Core.Message.Interface;
using Infraestructure.Message;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.DI;
public static class InfraestructureConfigurationExtension
{
    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("MassTransit:Host").Value;
        var user = configuration.GetSection("MassTransit:User").Value;
        var password = configuration.GetSection("MassTransit:Password").Value;

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            throw new Exception("Missing environment variables to configure MassTransit");

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(password);
                });
            });
        });

        // Register Message Publisher Abstraction
        services.AddScoped<IMessagePublisher, MessagePublisher>();
    }
}
