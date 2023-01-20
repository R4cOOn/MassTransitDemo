using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using MassTransit.Testing.Implementations;
using MassTransitDemo;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace SagaTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var correlationId = Guid.NewGuid();
            var id = 42;

            var busTestHarness = new InMemoryTestHarness();

            var mockRepository = new MockRepository(MockBehavior.Default);
            var loggerMock = mockRepository.Create<ILogger<MySagaStateMachine>>();

            var saga = new StateMachineSagaTestHarness<MyState, MySagaStateMachine>(
                busTestHarness,
                new InMemorySagaRepository<MyState>(),
                new MySagaStateMachine(loggerMock.Object),
                "test-queue");


            await busTestHarness.Start();

            // Initiate the Saga
            await busTestHarness.Bus.Publish(
                new MyCommand
                {
                    CorrelationId = correlationId,
                    Id = id,
                });
            (await busTestHarness.Consumed.Any<MyCommand>()).Should().BeTrue();

            (await saga.Consumed.Any<MyCommand>()).Should().BeTrue();

            (await saga.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

            var instance = saga.Created.ContainsInState(correlationId, saga.StateMachine, saga.StateMachine.Waiting);

            instance.Should().NotBeNull();
            instance.Id.Should().Be(id);
            instance.CorrelationId.Should().Be(correlationId);
            instance.CurrentState.Should().Be(saga.StateMachine.Waiting);

            await busTestHarness.Bus.Publish(new MyEvent(correlationId));
            (await busTestHarness.Consumed.Any<MyEvent>()).Should().BeTrue();
        }
    }
}