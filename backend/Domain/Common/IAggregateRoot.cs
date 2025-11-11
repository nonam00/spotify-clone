using System.Collections.ObjectModel;

namespace Domain.Common;

public interface IAggregateRoot
{
    ReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void CleanDomainEvents();
}