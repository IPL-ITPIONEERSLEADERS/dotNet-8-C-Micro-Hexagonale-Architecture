using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Aggregate
{
    public abstract class EventSourcedAggregate<TId>
    {
        private readonly List<IDomainEvent> _uncommittedEvents = new();

        public TId Id { get; protected set; } = default!;
        public int Version { get; protected set; }

        protected void Apply<TEvent>(TEvent @event) where TEvent : IDomainEvent
        {
            Mutate(@event);
            _uncommittedEvents.Add(@event);
            Version++;
        }

        protected abstract void Mutate(IDomainEvent @event);

        public IEnumerable<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents;
        public void ClearUncommittedEvents() => _uncommittedEvents.Clear();
        public void Replay(IEnumerable<IDomainEvent> events)
        {
            foreach (var @event in events)
            {
                Mutate(@event);
                Version++;
            }
        }
    }
}
