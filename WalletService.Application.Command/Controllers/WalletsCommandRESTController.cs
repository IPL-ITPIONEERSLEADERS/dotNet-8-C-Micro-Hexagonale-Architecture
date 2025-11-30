using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WalletService.Application.Command.Handlers;

namespace WalletService.Application.Command.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly CreateWalletCommandHandler _createCommandHandler;
       

        public WalletsController(
            CreateWalletCommandHandler createCommandHandler)
        {
            _createCommandHandler = createCommandHandler;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateWalletRequest request)
        {
            var walletId = await _createCommandHandler.Handle(new CreateWalletCommand(request.OwnerId, request.InitialBalance));
            return CreatedAtAction(nameof(Create), new { id = walletId.Value }, walletId.Value);
        }

      
    }

    public record CreateWalletRequest(Guid OwnerId, decimal InitialBalance);
}
