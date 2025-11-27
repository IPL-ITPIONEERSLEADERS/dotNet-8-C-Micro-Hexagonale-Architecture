using AccountService.Domain.Aggregates;
using AccountService.Domain.ValueObjects;
using AccountService.ES.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.ES.Impl
{

    public class AccountRepository : IAccountRepository
    {
        private readonly IEventStore _eventStore;
        private readonly List<AccountId> _allIds = new(); // Pour GetAll

        public AccountRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task SaveAsync(BankAccountAggregate account)
        {
            var id = account.Id;
            await _eventStore.SaveEventsAsync(id, account.GetUncommittedEvents());
            account.ClearUncommittedEvents();

            if (!_allIds.Contains(id))
                _allIds.Add(id);
        }

        public async Task<BankAccountAggregate?> GetByIdAsync(AccountId id)
        {
            var events = await _eventStore.GetEventsAsync(id);
            if (!events.Any()) return null;

            var account = new BankAccountAggregate();
            account.Replay(events);
            return account;
        }

        public async Task<List<BankAccountAggregate>> GetAllAsync()
        {
            var accounts = new List<BankAccountAggregate>();
            foreach (var id in _allIds)
            {
                var account = await GetByIdAsync(id);
                if (account != null)
                    accounts.Add(account);
            }
            return accounts;
        }
    }
}
