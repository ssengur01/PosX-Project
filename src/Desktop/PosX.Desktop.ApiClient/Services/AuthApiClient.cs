using System.Net.Http.Json;
using PosX.Desktop.ApiClient.Models;

namespace PosX.Desktop.ApiClient.Services;

public class AuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse?> LoginAsync(string emailOrUsername, string password)
    {
        var request = new LoginRequest(emailOrUsername, password);
        var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        return null;
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        var request = new { Token = refreshToken };
        var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        return null;
    }
}
