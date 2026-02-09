using Customers.Application.DTOs;
using MediatR;

namespace Customers.Application.Commands;

public record UpdateCustomerCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    string? Address
) : IRequest<CustomerDto?>;
