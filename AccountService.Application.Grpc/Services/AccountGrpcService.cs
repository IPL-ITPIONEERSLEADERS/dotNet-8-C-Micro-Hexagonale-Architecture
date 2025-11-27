using AccountService.Domain.Aggregates;
using AccountService.ES.Contracts;
using Grpc.Core;

namespace AccountService.Application.Grpc
{
    public class AccountGrpcService : AccountService.AccountServiceBase
    {
        // private readonly IAccountRepository _repository;

        public AccountGrpcService() //(IAccountRepository repository)
        {
            // _repository = repository;
        }

        public override Task<AccountResponse> GetAccountById(GetAccountRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid UUID"));

            // var account = await _repository.GetByIdAsync(new Domain.ValueObjects.AccountId(id));
            var account = new BankAccountAggregate();
            //
            if (account == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));

            return Task.FromResult(new AccountResponse
            {
                Id = request.Id,
                CustomerId = new Guid().ToString(),
                Balance = 300,
                IsClosed = false
                /*
                Id = account.Id.ToString(),
                CustomerId = account.GetCustomerId().ToString(),
                Balance = (double)account.GetBalance(),
                IsClosed = account.IsClosed()
                */
            });
        }
    }
}
