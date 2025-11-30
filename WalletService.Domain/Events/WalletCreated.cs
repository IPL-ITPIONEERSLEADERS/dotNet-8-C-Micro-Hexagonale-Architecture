using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletService.Domain.ValueObjects;

namespace WalletService.Domain.Events
{
    public record WalletCreated(
     WalletId WalletId,
     OwnerId OwnerId,
     decimal InitialBalance,
     DateTime Timestamp) : IDomainEvent;
}
