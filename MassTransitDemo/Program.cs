using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using MassTransit;
using MassTransitDemo;
using Microsoft.Extensions.Logging;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
        services.AddPublishSubscribe(Assembly.GetExecutingAssembly())
    )
    .Build();

var serviceScope = host.Services.CreateAsyncScope();
var serviceProvider = serviceScope.ServiceProvider;

var logger = serviceProvider.GetRequiredService<ILogger<object>>();
Logger.Set(logger);

var sagaSendPoint = await serviceProvider.GetRequiredService<ISendEndpointProvider>().GetSendEndpoint(new Uri("queue:my"));
var stateMachineSendPoint = await serviceProvider.GetRequiredService<ISendEndpointProvider>().GetSendEndpoint(new Uri("queue:my-state"));

for (int i = 1; i < 4; i++)
{
    await sagaSendPoint.Send(
        new MyCommand
        {
            CorrelationId = Guid.NewGuid(),
            Id = i,
        });

    await stateMachineSendPoint.Send(
        new MyCommand
        {
            CorrelationId = Guid.NewGuid(),
            Id = i * 10,
        });
}

await host.RunAsync();
