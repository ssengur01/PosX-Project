using Employees.Application.Commands;
using Employees.Application.DTOs;
using Employees.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Employees.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IMediator mediator, ILogger<EmployeesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetAllEmployeesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employees");
            return StatusCode(500, "An error occurred while retrieving employees");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetEmployeeByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound($"Employee with ID {id} not found");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee {EmployeeId}", id);
            return StatusCode(500, "An error occurred while retrieving the employee");
        }
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating employee");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, "An error occurred while creating the employee");
        }
    }

    [HttpPost("{id:guid}/clock-in")]
    public async Task<IActionResult> ClockIn(Guid id, [FromBody] ClockInRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new ClockInCommand(id, request.Notes);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound($"Employee with ID {id} not found");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while clocking in employee {EmployeeId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clocking in employee {EmployeeId}", id);
            return StatusCode(500, "An error occurred while clocking in");
        }
    }

    [HttpPost("{id:guid}/clock-out")]
    public async Task<IActionResult> ClockOut(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new ClockOutCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound($"Employee with ID {id} not found");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while clocking out employee {EmployeeId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clocking out employee {EmployeeId}", id);
            return StatusCode(500, "An error occurred while clocking out");
        }
    }
}

public record ClockInRequest(string? Notes);
