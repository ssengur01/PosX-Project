using MediatR;

namespace Customers.Application.Commands.AddLoyaltyPoints;

public record AddLoyaltyPointsCommand(Guid CustomerId, int Points) : IRequest<bool>;
