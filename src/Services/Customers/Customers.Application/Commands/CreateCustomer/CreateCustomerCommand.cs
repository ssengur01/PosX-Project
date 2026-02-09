using Customers.Application.DTOs;
using MediatR;

namespace Customers.Application.Commands.CreateCustomer;

public record CreateCustomerCommand(string FirstName, string LastName, string Phone, string? Email, string? Address) : IRequest<CustomerDto>;
