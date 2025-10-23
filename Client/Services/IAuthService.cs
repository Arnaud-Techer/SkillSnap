using Shared.Models;

namespace Client.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password, bool rememberMe = false);
    Task<AuthResult> RegisterAsync(string email, string password, string confirmPassword);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<ApplicationUser?> GetCurrentUserAsync();
    Task<bool> IsInRoleAsync(string role);
    Task InitializeAsync();
    event Action<bool>? AuthenticationStateChanged;
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
    public ApplicationUser? User { get; set; }
    public List<string> Errors { get; set; } = new();
}
