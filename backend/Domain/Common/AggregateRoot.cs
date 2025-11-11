using System.Collections.ObjectModel;

namespace Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
{
    private readonly List<DomainEvent> _domainEvents = [];
    public ReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() { }
    protected AggregateRoot(TId id) : base(id) { }

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void CleanDomainEvents()
    {
        _domainEvents.Clear();
    }
}