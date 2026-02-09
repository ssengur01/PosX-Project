using MediatR;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Commands.CreateProduct;
using Products.Application.Queries.GetProducts;

namespace Products.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetProductsQuery(page, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? CreatedAtAction(nameof(GetProducts), new { id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }
}
