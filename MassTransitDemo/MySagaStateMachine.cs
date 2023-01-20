using System.Security.Cryptography.X509Certificates;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitDemo
{
    public class MySagaStateMachine : MassTransitStateMachine<MyState>
    {
        public State Waiting { get; set; }

        public Event<MyCommand> MyCommand { get; private set; }
        public Event<MyEvent> MyEvent{ get; private set; }

        public MySagaStateMachine(ILogger<MySagaStateMachine> logger)
        {
            InstanceState(x => x.CurrentState);

            Event(() => MyCommand);
            Event(() => MyEvent);

            Initially(
                When(MyCommand)
                    .Then(
                        context =>
                        {
                            context.Saga.Id = context.Message.Id;
                            logger.LogInformation($"Entering state machine for Id {context.Message.Id}");
                        })
                    .TransitionTo(Waiting));

            During(
                Waiting,
                When(MyEvent)
                    .ThenAsync(
                        async context =>
                        {
                            logger.LogInformation("Processing MyEvent");
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        })
                    .TransitionTo(Final));

            Finally(
                binder => binder
                    .Then(context =>
                {
                    logger.LogInformation($"Leaving state machine for Id {context.Saga.Id}");
                }));

            SetCompletedWhenFinalized();
        }
    }

    public class MyState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public State CurrentState { get; set; }
        public int Id { get; set; }
    }
}
