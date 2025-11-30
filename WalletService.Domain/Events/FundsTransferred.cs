using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletService.Domain.ValueObjects;

namespace WalletService.Domain.Events
{
    public record FundsTransferred(
     WalletId SourceWalletId,
     WalletId TargetWalletId,
     decimal Amount,
     DateTime Timestamp) : IDomainEvent;
}
