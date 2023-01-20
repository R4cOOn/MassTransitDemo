using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;
using MassTransit;

namespace MassTransitDemo
{
    public static class Registrations
    {
        public static IServiceCollection AddPublishSubscribe(this IServiceCollection services, Assembly assembly)
        {
            return services
                    .Configure<HealthCheckPublisherOptions>(
                        options =>
                        {
                            options.Delay = TimeSpan.FromSeconds(2);
                            options.Predicate = check => check.Tags.Contains("ready", StringComparer.Ordinal);
                        })

                    .AddMassTransit(
                        m =>
                        {
                            m.AddConsumers(assembly);
                            m.AddSagaStateMachines(assembly);
                            m.AddSagas(assembly);
                            m.SetInMemorySagaRepositoryProvider();

                            m.SetKebabCaseEndpointNameFormatter();

                            m.UsingRabbitMq(
                                (context, configurator) =>
                                {
                                    configurator.Host(
                                        "localhost",
                                        "/",
                                        hostConfigurator =>
                                        {
                                            hostConfigurator.Username("guest");
                                            hostConfigurator.Password("guest");
                                        });

                                    configurator.ConfigureEndpoints(context);
                                    configurator.ConcurrentMessageLimit = 4;
                                });
                        })
                ;
        }
    }
}
