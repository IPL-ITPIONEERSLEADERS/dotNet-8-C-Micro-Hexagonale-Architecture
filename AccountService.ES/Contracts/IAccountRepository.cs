using AccountService.Domain.Aggregates;
using AccountService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.ES.Contracts
{
    public interface IAccountRepository
    {
        Task SaveAsync(BankAccountAggregate account);
        Task<BankAccountAggregate?> GetByIdAsync(AccountId id);
        Task<List<BankAccountAggregate>> GetAllAsync();
    }
}
