using AccountService.Domain.ValueObjects;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Events
{
    public record AccountClosed(
        AccountId AccountId,
        DateTime Timestamp) : IDomainEvent;
}
