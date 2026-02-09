using BuildingBlocks.Common.BaseClasses;

namespace Identity.Domain.Entities;

public class RefreshToken : Entity
{
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string CreatedByIp { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private RefreshToken() { } // EF Core

    public RefreshToken(string token, DateTime expiresAt, Guid userId, string createdByIp)
    {
        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
        CreatedByIp = createdByIp;
        IsRevoked = false;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string ipAddress, string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = ipAddress;
        ReplacedByToken = replacedByToken;
    }
}
