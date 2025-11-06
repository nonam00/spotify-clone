namespace Domain.Common;

public class DomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn{ get; }
    
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}