using MassTransit;

namespace MassTransitDemo
{
    public record MyCommand : CorrelatedBy<Guid>, ISaga
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
