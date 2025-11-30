using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletService.Application.Query.Handlers;

namespace WalletService.Application.Query.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsQueryRESTController : ControllerBase
    {
            private readonly GetWalletBalanceQueryHandler _getQueryHandler;

            public WalletsQueryRESTController(
                GetWalletBalanceQueryHandler getQueryHandler)
            {
                _getQueryHandler = getQueryHandler;
            }

            

            [HttpGet("{id:guid}")]
            public async Task<ActionResult<decimal?>> GetBalance(Guid id)
            {
                var balance = await _getQueryHandler.Handle(new GetWalletBalanceQuery(id));
                if (balance == null) return NotFound();
                return Ok(balance);
            }
    }    
}
