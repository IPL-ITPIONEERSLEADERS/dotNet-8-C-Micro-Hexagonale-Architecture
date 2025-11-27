using AccountService.Domain.Aggregates;
using AccountService.Domain.ValueObjects;
using AccountService.ES.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Application.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    // [Route("accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _repository;

        public AccountsController(IAccountRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountRequest request)
        {
            var account = BankAccountAggregate.Create(new CustomerId(request.CustomerId), request.InitialBalance);
            await _repository.SaveAsync(account);

            return CreatedAtAction(nameof(GetById), new { id = account.Id.Value }, ToDto(account));
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountDto>>> GetAll()
        {
            var accounts = await _repository.GetAllAsync();
            return accounts.Select(ToDto).ToList();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AccountDto>> GetById(Guid id)
        {
            var account = await _repository.GetByIdAsync(new AccountId(id));
            if (account == null) return NotFound();
            return ToDto(account);
        }

        private static AccountDto ToDto(BankAccountAggregate account) => new(
            account.Id.Value,
            account.GetCustomerId().Value,
            account.GetBalance(),
            account.IsClosed()
        );
    }

    public record CreateAccountRequest(Guid CustomerId, decimal InitialBalance);
    public record AccountDto(Guid Id, Guid CustomerId, decimal Balance, bool IsClosed);
}
