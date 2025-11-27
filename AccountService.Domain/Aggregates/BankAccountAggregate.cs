using AccountService.Domain.Events;
using AccountService.Domain.ValueObjects;
using Common.Aggregate;
using Common.Events;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Aggregates
{
    public class BankAccountAggregate : EventSourcedAggregate<AccountId>
    {
        private decimal _balance;
        private CustomerId _customerId;
        private bool _isClosed;

        public BankAccountAggregate() { }

        public static BankAccountAggregate Create(CustomerId customerId, decimal initialBalance)
        {
            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.");

            var account = new BankAccountAggregate();
            account.Apply(new AccountCreated(
                account.Id = AccountId.New(),
                customerId,
                initialBalance,
                DateTime.UtcNow
            ));
            return account;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.");
            if (_isClosed) throw new AccountClosedException();
            Apply(new FundsDeposited(Id, amount, DateTime.UtcNow));
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.");
            if (_isClosed) throw new AccountClosedException();
            if (_balance < amount) throw new InsufficientFundsException(_balance, amount);
            Apply(new FundsWithdrawn(Id, amount, DateTime.UtcNow));
        }

        public void Close()
        {
            if (_isClosed) return;
            if (_balance > 0) throw new InvalidOperationException("Cannot close account with balance.");
            Apply(new AccountClosed(Id, DateTime.UtcNow));
        }

        protected override void Mutate(IDomainEvent @event)
        {
            switch (@event)
            {
                case AccountCreated e:
                    Id = e.AccountId;
                    _customerId = e.CustomerId;
                    _balance = e.InitialBalance;
                    _isClosed = false;
                    break;
                case FundsDeposited e:
                    _balance += e.Amount;
                    break;
                case FundsWithdrawn e:
                    _balance -= e.Amount;
                    break;
                case AccountClosed _:
                    _isClosed = true;
                    break;
            }
        }

        public decimal GetBalance() => _balance;
        public CustomerId GetCustomerId() => _customerId;
        public bool IsClosed() => _isClosed;
    }
}
