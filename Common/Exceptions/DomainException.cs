using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(decimal balance, decimal amount)
            : base($"Insufficient funds: {balance} < {amount}") { }
    }

    public class AccountClosedException : Exception
    {
        public AccountClosedException() : base("Account is closed.") { }
    }
}
