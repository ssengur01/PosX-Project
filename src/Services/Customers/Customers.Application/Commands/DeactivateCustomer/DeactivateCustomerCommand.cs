using Customers.Application.DTOs;
using MediatR;

namespace Customers.Application.Commands;

public record DeactivateCustomerCommand(Guid Id) : IRequest<CustomerDto?>;
