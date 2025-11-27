using AccountService.Domain.ValueObjects;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.ES.Contracts
{

    public interface IEventStore
    {
        Task SaveEventsAsync(AccountId aggregateId, IEnumerable<IDomainEvent> events);
        Task<List<IDomainEvent>> GetEventsAsync(AccountId aggregateId);
    }
}
