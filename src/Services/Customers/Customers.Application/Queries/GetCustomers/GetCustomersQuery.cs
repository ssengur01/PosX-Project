using Customers.Application.DTOs;
using MediatR;

namespace Customers.Application.Queries.GetCustomers;

public record GetCustomersQuery(string? SearchTerm = null) : IRequest<List<CustomerDto>>;
