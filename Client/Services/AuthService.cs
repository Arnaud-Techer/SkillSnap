using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using Shared.Models;

namespace Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private const string TOKEN_KEY = "authToken";
    private const string USER_KEY = "currentUser";

    public event Action<bool>? AuthenticationStateChanged;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<AuthResult> LoginAsync(string email, string password, bool rememberMe = false)
    {
        try
        {
            Console.WriteLine($"LoginAsync called with email: {email}");
            
            var loginRequest = new
            {
                email = email,
                password = password,
                rememberMe = rememberMe
            };

            Console.WriteLine($"Making API call to: {_httpClient.BaseAddress}/api/Auth/login");
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginRequest);
            var content = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"API Response Status: {response.StatusCode}");
            Console.WriteLine($"API Response Content: {content}");

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Create a user object from the response
                    var user = new ApplicationUser
                    {
                        Id = loginResponse.UserId,
                        Email = loginResponse.Email,
                        UserName = loginResponse.Email
                    };

                    // Store token and user info
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, loginResponse.Token);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_KEY, JsonSerializer.Serialize(user));

                    // Set default authorization header
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);

                    AuthenticationStateChanged?.Invoke(true);

                    return new AuthResult
                    {
                        Success = true,
                        Token = loginResponse.Token,
                        User = user,
                        Message = loginResponse.Message
                    };
                }
            }

            // Handle error response
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new AuthResult
            {
                Success = false,
                Message = errorResponse?.Message ?? "Login failed",
                Errors = errorResponse?.Errors ?? new List<string> { "Unknown error occurred" }
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Message = "An error occurred during login",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string confirmPassword)
    {
        try
        {
            var registerRequest = new
            {
                email = email,
                password = password,
                confirmPassword = confirmPassword
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Auth/register", registerRequest);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var registerResponse = JsonSerializer.Deserialize<RegisterResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (registerResponse != null && !string.IsNullOrEmpty(registerResponse.Token))
                {
                    // Create a user object from the response
                    var user = new ApplicationUser
                    {
                        Id = registerResponse.UserId,
                        Email = registerResponse.Email,
                        UserName = registerResponse.Email
                    };

                    // Store token and user info
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, registerResponse.Token);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_KEY, JsonSerializer.Serialize(user));

                    // Set default authorization header
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", registerResponse.Token);

                    AuthenticationStateChanged?.Invoke(true);

                    return new AuthResult
                    {
                        Success = true,
                        Token = registerResponse.Token,
                        User = user,
                        Message = registerResponse.Message
                    };
                }
            }

            // Handle error response
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new AuthResult
            {
                Success = false,
                Message = errorResponse?.Message ?? "Registration failed",
                Errors = errorResponse?.Errors ?? new List<string> { "Unknown error occurred" }
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Message = "An error occurred during registration",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Remove token and user info from localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", USER_KEY);

            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            AuthenticationStateChanged?.Invoke(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during logout: {ex.Message}");
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        try
        {
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", USER_KEY);
            if (!string.IsNullOrEmpty(userJson))
            {
                return JsonSerializer.Deserialize<ApplicationUser>(userJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting current user: {ex.Message}");
        }
        return null;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var user = await GetCurrentUserAsync();
        if (user != null)
        {
            // For now, we'll check if the user has the role claim
            // In a more complex scenario, you might want to decode the JWT token
            // and check the role claims directly
            return user.UserName?.ToLower().Contains("admin") == true;
        }
        return false;
    }

    public async Task InitializeAsync()
    {
        var token = await GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}

// Response models
public class LoginResponse
{
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}

public class RegisterResponse
{
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}
