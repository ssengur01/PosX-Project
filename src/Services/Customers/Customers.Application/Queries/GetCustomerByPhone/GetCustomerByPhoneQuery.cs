using Customers.Application.DTOs;
using MediatR;

namespace Customers.Application.Queries.GetCustomerByPhone;

public record GetCustomerByPhoneQuery(string Phone) : IRequest<CustomerDto?>;
