using CardService.Domain.ValueObjects;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Infrastructure.Contracts
{
    public interface IEventStore
    {
        Task SaveEventsAsync(CardId aggregateId, IEnumerable<IDomainEvent> events);
        Task<List<IDomainEvent>> GetEventsAsync(CardId aggregateId);
    }
}
