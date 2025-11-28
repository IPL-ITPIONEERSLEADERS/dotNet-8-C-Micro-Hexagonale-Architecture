using CardService.Domain.Entities;
using CardService.Domain.Events;
using CardService.Domain.ValueObjects;
using Common.Aggregate;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Domain.Aggregates
{
    public class BankCardAggregate : EventSourcedAggregate<CardId>
    {
        private CardDetails _details = null!;
        private OwnerId _ownerId;
        private string _ownerFirstName = string.Empty;
        private string _ownerLastName = string.Empty;
        private AccountId _accountId;
        private string _status = "Issued"; // Issued, Active, Blocked, Expired
        private decimal _availableBalance; // mis à jour via intégration

        public BankCardAggregate() { }

        public static BankCardAggregate Issue(
            OwnerId ownerId,
            string firstName,
            string lastName,
            PhoneNumber phone,
            string cardNumber,
            DateOnly expiryDate,
            string cvv,
            AccountId accountId)
        {
            var details = new CardDetails(cardNumber, expiryDate, cvv);
            var card = new BankCardAggregate();

            card.Apply(new CardIssued(
                card.Id = CardId.New(),
                ownerId,
                details.CardNumber,
                details.ExpiryDate,
                details.Cvv,
                accountId,
                firstName,
                lastName,
                phone,
                DateTime.UtcNow
            ));
            return card;
        }

        public void Activate()
        {
            if (_status != "Issued")
                throw new InvalidOperationException("Only issued cards can be activated.");
            Apply(new CardActivated(Id, DateTime.UtcNow));
        }

        public void Block(string reason)
        {
            if (_status == "Blocked" || _status == "Expired")
                return;
            Apply(new CardBlocked(Id, reason, DateTime.UtcNow));
        }

        public void ProcessPayment(
            string merchantName,
            string merchantCategory,
            decimal amount,
            decimal currentAccountBalance) // ← obtenu via gRPC
        {
            EnsureCardIsValid();
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be positive.");
            if (currentAccountBalance < amount)
                throw new InvalidOperationException("Insufficient funds in linked account.");

            Apply(new PaymentProcessed(Id, amount, merchantName, merchantCategory, DateTime.UtcNow));
        }

        public void WithdrawCash(
            string atmLocation,
            decimal amount,
            decimal currentAccountBalance) // ← obtenu via gRPC
        {
            EnsureCardIsValid();
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be positive.");
            if (currentAccountBalance < amount)
                throw new InvalidOperationException("Insufficient funds in linked account.");

            Apply(new CashWithdrawn(Id, amount, atmLocation, DateTime.UtcNow));
        }

        private void EnsureCardIsValid()
        {
            if (_status == "Blocked")
                throw new InvalidOperationException("Card is blocked.");
            if (_details.IsExpired())
                throw new InvalidOperationException("Card is expired.");
        }

        protected override void Mutate(IDomainEvent @event)
        {
            switch (@event)
            {
                case CardIssued e:
                    Id = e.CardId;
                    _ownerId = e.OwnerId;
                    _ownerFirstName = e.OwnerFirstName;
                    _ownerLastName = e.OwnerLastName;
                    _details = new CardDetails(e.CardNumber, e.ExpiryDate, e.Cvv);
                    _accountId = e.AccountId;
                    _status = "Issued";
                    break;
                case CardActivated _:
                    _status = "Active";
                    break;
                case CardBlocked _:
                    _status = "Blocked";
                    break;
                case PaymentProcessed e:
                    // On ne stocke pas l'historique ici (Event Sourcing → les événements le sont)
                    break;
                case CashWithdrawn e:
                    // Idem
                    break;
            }
        }

        public string GetStatus() => _status;
        public AccountId GetAccountId() => _accountId;
        public bool IsBlocked() => _status == "Blocked";
        public bool IsExpired() => _details.IsExpired();
    }
}
