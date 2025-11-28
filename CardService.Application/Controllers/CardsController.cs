using CardService.Domain.Aggregates;
using CardService.Domain.ValueObjects;
using CardService.Infrastructure.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CardService.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardRepository _repository;

        public CardsController(ICardRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult<CardDto>> Issue([FromBody] IssueCardRequest request)
        {
            var card = BankCardAggregate.Issue(
                new OwnerId(request.OwnerId),
                request.FirstName,
                request.LastName,
                new PhoneNumber(request.Phone),
                request.CardNumber,
                DateOnly.FromDateTime(request.ExpiryDate),
                request.Cvv,
                new AccountId(request.AccountId)
            );
            await _repository.SaveAsync(card);
            return CreatedAtAction(nameof(GetById), new { id = card.Id.Value }, ToDto(card));
        }

        [HttpPost("{id:guid}/block")]
        public async Task<ActionResult> Block(Guid id, [FromBody] BlockCardRequest request)
        {
            var card = await _repository.GetByIdAsync(new CardId(id));
            if (card == null) return NotFound();
            card.Block(request.Reason);
            await _repository.SaveAsync(card);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<CardDto>>> GetAll()
        {
            var cards = await _repository.GetAllAsync();
            return cards.Select(ToDto).ToList();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CardDto>> GetById(Guid id)
        {
            var card = await _repository.GetByIdAsync(new CardId(id));
            if (card == null) return NotFound();
            return ToDto(card);
        }

        private static CardDto ToDto(BankCardAggregate card) => new(
            card.Id.Value,
            card.GetAccountId().Value,
            card.GetStatus()
        );
    }

    public record IssueCardRequest(
        Guid OwnerId,
        string FirstName,
        string LastName,
        string Phone,
        string CardNumber,
        DateTime ExpiryDate,
        string Cvv,
        Guid AccountId
    );

    public record BlockCardRequest(string Reason);
    public record CardDto(Guid Id, Guid AccountId, string Status);
}
