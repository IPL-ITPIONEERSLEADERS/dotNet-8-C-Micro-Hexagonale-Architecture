using Common.Events;
using EventStore.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WalletService.Domain.Events;
using WalletService.Domain.ValueObjects;
using WalletService.Infrastructure.EventStore.Contracts;

namespace WalletService.Infrastructure.EventStore.Impl
{
    public class EventStoreDBEventStore : IEventStore
    {
        private readonly EventStoreClient _client;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public EventStoreDBEventStore(EventStoreClient client) => _client = client;

        public async Task SaveEventsAsync(WalletId aggregateId, IEnumerable<IDomainEvent> events)
        {
            var streamId = $"wallet-{aggregateId.Value}";
            var eventDataList = events.Select(ev => new EventData(
                Uuid.NewUuid(),
                ev.GetType().Name,
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ev, ev.GetType(), JsonOptions))
            )).ToList();

            await _client.AppendToStreamAsync(streamId, StreamState.Any, eventDataList);
        }


        public async Task SubscribeToAllEvents(Func<IDomainEvent, Task> eventHandler)
        {
            var subscription = _client.SubscribeToAllAsync(
                FromAll.Start,
                async (subscription, evt, token) =>
                {
                    if (evt.OriginalEvent.EventStreamId.StartsWith("wallet-"))
                    {
                        // ✅ CORRIGÉ : utilisez OriginalEvent partout
                        var eventType = evt.OriginalEvent.EventType;
                        var jsonData = Encoding.UTF8.GetString(evt.OriginalEvent.Data.Span);

                        var assembly = typeof(WalletCreated).Assembly;
                        var eventTypeClass = assembly.GetType($"WalletService.Domain.Events.{eventType}");
                        if (eventTypeClass != null)
                        {
                            var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(jsonData, eventTypeClass, JsonOptions);
                            if (domainEvent != null)
                                await eventHandler(domainEvent);
                        }
                    }
                    // ❌ Supprimez cette ligne → inutile et cause CS8031
                    // return await Task.CompletedTask;
                });
        }


    }
}
