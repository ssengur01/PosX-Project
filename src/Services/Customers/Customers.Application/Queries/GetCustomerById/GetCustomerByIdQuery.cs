using Customers.Application.DTOs;
using MediatR;

namespace Customers.Application.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto?>;
