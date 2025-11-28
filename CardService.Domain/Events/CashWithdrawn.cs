using CardService.Domain.ValueObjects;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Domain.Events
{
    public record CashWithdrawn(
    CardId CardId,
    decimal Amount,
    string AtmLocation,
    DateTime Timestamp) : IDomainEvent;
}
