using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletService.Domain.ValueObjects
{
    public record WalletId(Guid Value)
    {
        public static WalletId New() => new(Guid.NewGuid());
    }
}
