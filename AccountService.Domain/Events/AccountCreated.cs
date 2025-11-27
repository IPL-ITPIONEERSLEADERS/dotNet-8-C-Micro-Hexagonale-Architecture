using AccountService.Domain.ValueObjects;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Events
{
     public record AccountCreated(
        AccountId AccountId,
        CustomerId CustomerId,
        decimal InitialBalance,
        DateTime Timestamp) : IDomainEvent;
}
