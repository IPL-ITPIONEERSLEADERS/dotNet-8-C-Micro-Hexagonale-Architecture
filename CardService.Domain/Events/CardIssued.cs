using CardService.Domain.ValueObjects;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Domain.Events
{
    public record CardIssued(
    CardId CardId,
    OwnerId OwnerId,
    string CardNumber,
    DateOnly ExpiryDate,
    string Cvv,
    AccountId AccountId,
    string OwnerFirstName,
    string OwnerLastName,
    PhoneNumber Phone,
    DateTime Timestamp) : IDomainEvent;


}
