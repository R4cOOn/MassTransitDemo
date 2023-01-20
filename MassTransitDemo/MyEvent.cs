using MassTransit;

namespace MassTransitDemo;

public record MyEvent(Guid CorrelationId) : CorrelatedBy<Guid>
{
}