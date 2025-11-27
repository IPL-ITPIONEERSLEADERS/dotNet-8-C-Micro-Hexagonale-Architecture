using AccountService.Domain.ValueObjects;
using AccountService.ES.Contracts;
using Common.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.ES.Impl
{

    public class InMemoryEventStore : IEventStore
    {
        private readonly ConcurrentDictionary<Guid, List<IDomainEvent>> _store = new();

        public Task SaveEventsAsync(AccountId aggregateId, IEnumerable<IDomainEvent> events)
        {
            var eventList = events.ToList();
            _store.AddOrUpdate(aggregateId.Value,
                _ => eventList,
                (_, existing) => { existing.AddRange(eventList); return existing; });
            return Task.CompletedTask;
        }

        public Task<List<IDomainEvent>> GetEventsAsync(AccountId aggregateId)
        {
            if (_store.TryGetValue(aggregateId.Value, out var events))
                return Task.FromResult(new List<IDomainEvent>(events));
            return Task.FromResult(new List<IDomainEvent>());
        }
    }
}
