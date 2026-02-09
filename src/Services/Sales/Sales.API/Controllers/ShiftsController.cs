using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Commands.AddCashMovement;
using Sales.Application.Commands.CloseShift;
using Sales.Application.Commands.OpenShift;
using Sales.Application.Queries.GetOpenShift;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShiftsController> _logger;

    public ShiftsController(IMediator mediator, ILogger<ShiftsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenShift([FromBody] OpenShiftRequest request)
    {
        try
        {
            var command = new OpenShiftCommand(request.EmployeeId, request.StartingCash, request.Currency);
            var shiftId = await _mediator.Send(command);
            return Ok(new { shiftId });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot open shift for employee {EmployeeId}", request.EmployeeId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening shift for employee {EmployeeId}", request.EmployeeId);
            return StatusCode(500, new { error = "An error occurred while opening shift" });
        }
    }

    [HttpPost("{id}/close")]
    public async Task<IActionResult> CloseShift(Guid id, [FromBody] CloseShiftRequest request)
    {
        try
        {
            var command = new CloseShiftCommand(id, request.EndingCash, request.Currency, request.Notes);
            await _mediator.Send(command);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot close shift {ShiftId}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing shift {ShiftId}", id);
            return StatusCode(500, new { error = "An error occurred while closing shift" });
        }
    }

    [HttpPost("{id}/cash-movement")]
    public async Task<IActionResult> AddCashMovement(Guid id, [FromBody] AddCashMovementRequest request)
    {
        try
        {
            var command = new AddCashMovementCommand(id, request.Type, request.Amount, request.Currency, request.Reason);
            await _mediator.Send(command);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot add cash movement to shift {ShiftId}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding cash movement to shift {ShiftId}", id);
            return StatusCode(500, new { error = "An error occurred while adding cash movement" });
        }
    }

    [HttpGet("open/employee/{employeeId}")]
    public async Task<IActionResult> GetOpenShift(Guid employeeId)
    {
        try
        {
            var shift = await _mediator.Send(new GetOpenShiftQuery(employeeId));
            return shift == null ? NotFound() : Ok(shift);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting open shift for employee {EmployeeId}", employeeId);
            return StatusCode(500, new { error = "An error occurred while retrieving shift" });
        }
    }
}

public record OpenShiftRequest(Guid EmployeeId, decimal StartingCash, string Currency);
public record CloseShiftRequest(decimal EndingCash, string Currency, string? Notes);
public record AddCashMovementRequest(string Type, decimal Amount, string Currency, string Reason);
