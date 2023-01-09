using MassTransit;

namespace MassTransitDemo
{
    public class MySaga : ISaga, InitiatedBy<MyCommand>
    {
        public Guid CorrelationId { get; set; }

        public async Task Consume(ConsumeContext<MyCommand> context)
        {
            Logger.Log($"Entering the Saga with command Id {context.Message.Id}");

            await Task.Delay(TimeSpan.FromSeconds(5));

            Logger.Log($"Leaving the Saga with command Id {context.Message.Id}");
        }
    }
}
