using Inventory.Application.Commands.AdjustStock;
using Inventory.Application.Queries.GetStockLevels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetStockLevels([FromQuery] Guid? productId)
    {
        var result = await _mediator.Send(new GetStockLevelsQuery(productId));
        return Ok(result);
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> AdjustStock([FromBody] AdjustStockCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { success = result });
    }
}
