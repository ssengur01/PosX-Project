using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Commands.CreateSale;
using Sales.Application.Commands.ProcessRefund;
using Sales.Application.Queries.GetSaleById;
using Sales.Application.Queries.GetSales;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesController> _logger;

    public SalesController(IMediator mediator, ILogger<SalesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sale = await _mediator.Send(new GetSaleByIdQuery(id));
        return sale == null ? NotFound() : Ok(sale);
    }

    [HttpGet]
    public async Task<IActionResult> GetSales([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] Guid? employeeId)
    {
        var sales = await _mediator.Send(new GetSalesQuery(startDate, endDate, employeeId));
        return Ok(sales);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command)
    {
        try
        {
            var sale = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/refund")]
    public async Task<IActionResult> ProcessRefund(Guid id, [FromBody] ProcessRefundRequest request)
    {
        try
        {
            var command = new ProcessRefundCommand(
                id,
                request.Amount,
                request.Currency,
                request.Reason,
                request.ProcessedBy
            );
            var refundId = await _mediator.Send(command);
            return Ok(new { refundId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for sale {SaleId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record ProcessRefundRequest(
    decimal Amount,
    string Currency,
    string Reason,
    string ProcessedBy
);
