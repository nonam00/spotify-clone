using System.Collections.ObjectModel;

namespace Domain.Common;

internal interface IAggregateRoot
{
    ReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void CleanDomainEvents();
}