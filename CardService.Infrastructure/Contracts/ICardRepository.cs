using CardService.Domain.Aggregates;
using CardService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Infrastructure.Contracts
{
    public interface ICardRepository
    {
        Task SaveAsync(BankCardAggregate card);
        Task<BankCardAggregate?> GetByIdAsync(CardId id);
        Task<List<BankCardAggregate>> GetAllAsync();
    }
}
