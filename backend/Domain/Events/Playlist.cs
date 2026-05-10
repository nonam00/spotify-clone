using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public record PlaylistImageChangedEvent(Guid PlaylistId, FilePath OldImagePath) : DomainEvent;

public record PlaylistDeletedEvent(Guid PlaylistId, FilePath ImagePath) : DomainEvent;