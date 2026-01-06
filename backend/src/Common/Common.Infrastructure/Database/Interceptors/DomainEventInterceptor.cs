using Common.Abstractions.Domain;
using Common.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Common.Infrastructure.Database.Interceptors;

public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventInterceptor(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        while (true)
        {
            var aggregates = dbContext.ChangeTracker
                .Entries<IAggregateRoot>()
                .Where(entry => entry.Entity.DomainEvents.Count != 0)
                .Select(entry => entry.Entity)
                .ToList();

            if (aggregates.Count == 0)
                break;

            var aggregateDomainEvents = aggregates
                .SelectMany(aggregate => aggregate.DomainEvents)
                .ToList();

            aggregates.ForEach(aggregate => aggregate.ClearDomainEvents());

            if (aggregateDomainEvents.Count != 0)
                await _dispatcher.DispatchAsync(aggregateDomainEvents, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}