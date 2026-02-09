using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, List<string> roles);
    string GenerateRefreshToken();
}
