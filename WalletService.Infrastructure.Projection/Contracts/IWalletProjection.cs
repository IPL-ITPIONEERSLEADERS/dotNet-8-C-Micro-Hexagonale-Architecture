using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletService.Domain.ValueObjects;

namespace WalletService.Infrastructure.Projection.Contracts
{
    public interface IWalletProjection
    {
        Task UpdateBalance(WalletId walletId, decimal balance);
        Task<decimal?> GetBalance(WalletId walletId);
    }
}
