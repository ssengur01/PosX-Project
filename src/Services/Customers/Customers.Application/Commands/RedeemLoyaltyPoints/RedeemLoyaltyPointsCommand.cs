using Customers.Application.DTOs;
using MediatR;

namespace Customers.Application.Commands;

public record RedeemLoyaltyPointsCommand(
    Guid CustomerId,
    int Points
) : IRequest<CustomerDto?>;
