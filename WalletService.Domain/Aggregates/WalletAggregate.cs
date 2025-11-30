using Common.Aggregate;
using Common.Events;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletService.Domain.Events;
using WalletService.Domain.ValueObjects;

namespace WalletService.Domain.Aggregates
{
    public class WalletAggregate : EventSourcedAggregate<WalletId>
    {
        private decimal _balance;
        private OwnerId _ownerId;

        private WalletAggregate() { }

        public static WalletAggregate Create(OwnerId ownerId, decimal initialBalance)
        {
            if (initialBalance < 0)
                throw new ArgumentException("Initial balance must be >= 0.");
            var wallet = new WalletAggregate();
            wallet.Apply(new WalletCreated(wallet.Id = WalletId.New(), ownerId, initialBalance, DateTime.UtcNow));
            return wallet;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be > 0.");
            Apply(new FundsDeposited(Id, amount, DateTime.UtcNow));
        }

        public void TransferTo(WalletAggregate target, decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be > 0.");
            if (_balance < amount) throw new InsufficientFundsException(_balance, amount);
            Apply(new FundsTransferred(Id, target.Id, amount, DateTime.UtcNow));
            target.Apply(new FundsDeposited(target.Id, amount, DateTime.UtcNow));
        }

        protected override void Mutate(IDomainEvent @event)
        {
            switch (@event)
            {
                case WalletCreated e:
                    _ownerId = e.OwnerId;
                    _balance = e.InitialBalance;
                    break;
                case FundsDeposited e:
                    _balance += e.Amount;
                    break;
                case FundsTransferred e when e.SourceWalletId.Equals(Id):
                    _balance -= e.Amount;
                    break;
            }
        }

        public decimal GetBalance() => _balance;
    }
}
