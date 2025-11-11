using Microsoft.Extensions.Logging;

using Domain.Common;
using Application.Shared.Data;

namespace Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
{
    private readonly SongsDbContext _context;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(SongsDbContext context, IDomainEventDispatcher domainEventDispatcher, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _domainEventDispatcher = domainEventDispatcher;
        _logger = logger;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);
        
        // Dispatch domain events after successful save
        await DispatchDomainEventsAsync(cancellationToken).ConfigureAwait(false);
        
        return result;
    }
    
    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .Where(x => x.Entity.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.CleanDomainEvents());
        
        if (domainEvents.Count != 0)
        {
            _logger.LogDebug("Dispatching {EventCount} domain events", domainEvents.Count);
            
            foreach (var domainEvent in domainEvents)
            {
                _logger.LogDebug("Dispatching domain event: {EventType} - {EventId}", 
                    domainEvent.GetType().Name, domainEvent.EventId);
            }
            
            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}