using CardService.Domain.ValueObjects;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Domain.Events
{
    public record PaymentProcessed(
    CardId CardId,
    decimal Amount,
    string MerchantName,
    string MerchantCategory,
    DateTime Timestamp) : IDomainEvent;
}
