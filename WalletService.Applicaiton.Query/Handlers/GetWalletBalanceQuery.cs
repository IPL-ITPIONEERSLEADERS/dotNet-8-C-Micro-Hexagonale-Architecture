using WalletService.Domain.ValueObjects;
using WalletService.Infrastructure.Projection.Contracts;

namespace WalletService.Application.Query.Handlers
{
    public record GetWalletBalanceQuery(Guid WalletId);

    public class GetWalletBalanceQueryHandler
    {
        private readonly IWalletProjection _projection;

        public GetWalletBalanceQueryHandler(IWalletProjection projection) => _projection = projection;

        public async Task<decimal?> Handle(GetWalletBalanceQuery query)
        {
            return await _projection.GetBalance(new WalletId(query.WalletId));
        }
    }
}
