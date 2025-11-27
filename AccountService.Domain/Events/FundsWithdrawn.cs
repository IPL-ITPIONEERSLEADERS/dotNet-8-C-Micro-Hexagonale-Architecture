using AccountService.Domain.ValueObjects;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Events
{
   public record FundsWithdrawn(
        AccountId AccountId,
        decimal Amount,
        DateTime Timestamp) : IDomainEvent;
}
