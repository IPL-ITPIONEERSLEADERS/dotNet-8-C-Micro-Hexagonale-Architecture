using CardService.Domain.ValueObjects;
using CardService.Infrastructure.Contracts;
using Common.Events;
using EventStore.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CardService.Infrastructure.Impl
{

    public class EventStoreDBEventStore : IEventStore
    {
        private readonly EventStoreClient _client;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public EventStoreDBEventStore(EventStoreClient client)
        {
            _client = client;
        }

        public async Task SaveEventsAsync(CardId aggregateId, IEnumerable<IDomainEvent> events)
        {
            var streamId = $"card-{aggregateId.Value}";
            var eventDataList = events.Select(ev =>
            {
                var eventType = ev.GetType().Name;
                var jsonData = JsonSerializer.Serialize(ev, ev.GetType(), JsonOptions);
                return new EventData(Uuid.NewUuid(), eventType, Encoding.UTF8.GetBytes(jsonData));
            }).ToList();

            await _client.AppendToStreamAsync(streamId, StreamState.Any, eventDataList);
        }

        public async Task<List<IDomainEvent>> GetEventsAsync(CardId aggregateId)
        {
            var streamId = $"card-{aggregateId.Value}";
            var events = _client.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start);
            var domainEvents = new List<IDomainEvent>();

            // 🔑 Utiliser le bon assembly
            var domainAssembly = typeof(CardService.Domain.Events.CardIssued).Assembly;

            await foreach (var resolvedEvent in events)
            {
                var eventType = resolvedEvent.Event.EventType;
                var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);

                // 🔍 Chercher dans le bon assembly
                var eventTypeClass = domainAssembly.GetType($"CardService.Domain.Events.{eventType}");

                if (eventTypeClass != null)
                {
                    var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(jsonData, eventTypeClass, JsonOptions);
                    if (domainEvent != null)
                        domainEvents.Add(domainEvent);
                }
                else
                {
                    // 🔍 Aide au debug
                    Console.WriteLine($"⚠️ Type d'événement non trouvé : {eventType}");
                }
            }
            return domainEvents;
            /*
            var streamId = $"card-{aggregateId.Value}";
            var events = _client.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start);
            var domainEvents = new List<IDomainEvent>();
            await foreach (var resolvedEvent in events)
            {
                var eventType = resolvedEvent.Event.EventType;
                var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
                var assembly = typeof(IDomainEvent).Assembly;
                var eventTypeClass = assembly.GetType($"CardService.Domain.Events.{eventType}");
                if (eventTypeClass != null)
                {
                    var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(jsonData, eventTypeClass, JsonOptions);
                    if (domainEvent != null)
                        domainEvents.Add(domainEvent);
                }
            }
            return domainEvents;
            */
        }
    }
}