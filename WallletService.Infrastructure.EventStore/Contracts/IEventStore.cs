using Common.Events;
using EventStore.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WalletService.Domain.ValueObjects;

namespace WalletService.Infrastructure.EventStore.Contracts
{
    public interface IEventStore
    {
        Task SaveEventsAsync(WalletId aggregateId, IEnumerable<IDomainEvent> events);

        Task SubscribeToAllEvents(Func<IDomainEvent, Task> eventHandler);

    }
}
