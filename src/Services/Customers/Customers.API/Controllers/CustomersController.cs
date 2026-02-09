using Customers.Application.Commands;
using Customers.Application.Commands.CreateCustomer;
using Customers.Application.Commands.AddLoyaltyPoints;
using Customers.Application.DTOs;
using Customers.Application.Queries.GetCustomers;
using Customers.Application.Queries.GetCustomerById;
using Customers.Application.Queries.GetCustomerByPhone;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Customers.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerDto>>> GetAll([FromQuery] string? searchTerm, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomersQuery(searchTerm);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers. SearchTerm: {SearchTerm}", searchTerm);
            return StatusCode(500, "An error occurred while retrieving customers");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomerByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound($"Customer with ID {id} not found");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer {CustomerId}", id);
            return StatusCode(500, "An error occurred while retrieving the customer");
        }
    }

    [HttpGet("phone/{phone}")]
    public async Task<ActionResult<CustomerDto>> GetByPhone(string phone, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetCustomerByPhoneQuery(phone);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound($"Customer with phone {phone} not found");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer by phone {Phone}", phone);
            return StatusCode(500, "An error occurred while retrieving the customer");
        }
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating customer");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, "An error occurred while creating the customer");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command, cancellationToken);

            if (result == null)
                return NotFound($"Customer with ID {id} not found");

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating customer {CustomerId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return StatusCode(500, "An error occurred while updating the customer");
        }
    }

    [HttpPost("{id:guid}/loyalty-points")]
    public async Task<ActionResult> AddLoyaltyPoints(
        Guid id,
        [FromBody] AddLoyaltyPointsCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            if (id != command.CustomerId)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound($"Customer with ID {id} not found");

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while adding loyalty points for customer {CustomerId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding loyalty points for customer {CustomerId}", id);
            return StatusCode(500, "An error occurred while adding loyalty points");
        }
    }

    [HttpPost("{id:guid}/redeem-points")]
    public async Task<ActionResult<CustomerDto>> RedeemLoyaltyPoints(
        Guid id,
        [FromBody] RedeemLoyaltyPointsCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            if (id != command.CustomerId)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command, cancellationToken);

            if (result == null)
                return NotFound($"Customer with ID {id} not found");

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while redeeming points for customer {CustomerId}", id);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while redeeming points for customer {CustomerId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redeeming loyalty points for customer {CustomerId}", id);
            return StatusCode(500, "An error occurred while redeeming loyalty points");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeactivateCustomerCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (result == null)
                return NotFound($"Customer with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating customer {CustomerId}", id);
            return StatusCode(500, "An error occurred while deactivating the customer");
        }
    }
}
