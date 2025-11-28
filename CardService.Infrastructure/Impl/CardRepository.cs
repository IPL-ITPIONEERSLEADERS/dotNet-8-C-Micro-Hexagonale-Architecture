using CardService.Domain.Aggregates;
using CardService.Domain.ValueObjects;
using CardService.Infrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Infrastructure.Impl
{
    public class CardRepository : ICardRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly List<CardId> _allIds = new();

        public CardRepository(IEventStore eventStore, IRabbitMqPublisher rabbitMqPublisher)
        {
            _eventStore = eventStore;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task SaveAsync(BankCardAggregate card)
        {
            /*
            // 1. Persistance synchrone
            await _eventStore.SaveEventsAsync(card.Id, card.GetUncommittedEvents());
            card.ClearUncommittedEvents();

            // 2. Publication asynchrone vers RabbitMQ
            foreach (var ev in card.GetUncommittedEvents().ToList()) // copie car Clear() a été appelé
                await _rabbitMqPublisher.PublishEventAsync(ev);
            */
            // 1. Persistance synchrone
            await _eventStore.SaveEventsAsync(card.Id, card.GetUncommittedEvents());

            // 2. Publication asynchrone → AVANT Clear()
            foreach (var ev in card.GetUncommittedEvents()) // ← pas besoin de ToList() ici
                await _rabbitMqPublisher.PublishEventAsync(ev);

            // 3. Nettoyage
            card.ClearUncommittedEvents();

            if (!_allIds.Contains(card.Id))
                _allIds.Add(card.Id);
        }

        public async Task<BankCardAggregate?> GetByIdAsync(CardId id)
        {
            var events = await _eventStore.GetEventsAsync(id);
            if (!events.Any()) return null;
            var card = new BankCardAggregate();
            card.Replay(events);
            return card;
        }

        public async Task<List<BankCardAggregate>> GetAllAsync()
        {
            var cards = new List<BankCardAggregate>();
            foreach (var id in _allIds)
            {
                var card = await GetByIdAsync(id);
                if (card != null)
                    cards.Add(card);
            }
            return cards;
        }
    }
}
