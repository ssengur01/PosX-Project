using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace PosX.Desktop.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsStringAsync("accessToken");

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(_anonymous);

            var userEmail = await _localStorage.GetItemAsStringAsync("userEmail");
            var userName = await _localStorage.GetItemAsStringAsync("userName");
            var userRoles = await _localStorage.GetItemAsync<List<string>>("userRoles") ?? new List<string>();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userEmail ?? ""),
                new Claim(ClaimTypes.Name, userName ?? "")
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task MarkUserAsAuthenticated(string email, string userName, List<string> roles, string accessToken, string refreshToken)
    {
        await _localStorage.SetItemAsStringAsync("accessToken", accessToken);
        await _localStorage.SetItemAsStringAsync("refreshToken", refreshToken);
        await _localStorage.SetItemAsStringAsync("userEmail", email);
        await _localStorage.SetItemAsStringAsync("userName", userName);
        await _localStorage.SetItemAsync("userRoles", roles);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, userName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("accessToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        await _localStorage.RemoveItemAsync("userEmail");
        await _localStorage.RemoveItemAsync("userName");
        await _localStorage.RemoveItemAsync("userRoles");

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
