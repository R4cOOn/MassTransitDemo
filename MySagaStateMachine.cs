using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitDemo
{
    public class MySagaStateMachine : MassTransitStateMachine<MyState>
    {
        public Event<MyCommand> MyCommand { get; private set; }

        public MySagaStateMachine(ILogger<MySagaStateMachine> logger)
        {
            InstanceState(x => x.CurrentState);

            Event(() => MyCommand);

            Initially(
                When(MyCommand)
                    .ThenAsync(
                    async context =>
                    {
                        context.Saga.Id = context.Message.Id;
                        logger.LogInformation($"Entering state machine for Id {context.Message.Id}");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    })
                    .TransitionTo(Final));

            Finally(
                binder => binder
                    .Then(context =>
                {
                    logger.LogInformation($"Leaving state machine for Id {context.Saga.Id}");
                }));
        }
    }

    public class MyState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public State CurrentState { get; set; }
        public int Id { get; set; }
    }
}
