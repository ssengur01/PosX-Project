namespace PosX.Desktop.ApiClient.Models;

public record LoginRequest(string EmailOrUsername, string Password);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User);

public record UserDto(
    Guid Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    List<string> Roles);
