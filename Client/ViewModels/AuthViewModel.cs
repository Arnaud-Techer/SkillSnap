using Client.Services;

namespace Client.ViewModels;

public class AuthViewModel
{
    private readonly IAuthService _authService;

    public event Action<bool>? AuthenticationStateChanged;

    public AuthViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    // Login properties
    public string LoginEmail { get; set; } = string.Empty;
    public string LoginPassword { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
    public bool IsLoggingIn { get; set; } = false;
    public string? LoginErrorMessage { get; set; }

    // Register properties
    public string RegisterEmail { get; set; } = string.Empty;
    public string RegisterPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public bool IsRegistering { get; set; } = false;
    public string? RegisterErrorMessage { get; set; }

    // General properties
    public bool IsAuthenticated { get; set; } = false;
    public string? CurrentUserEmail { get; set; }

    public async Task<bool> LoginAsync()
    {
        Console.WriteLine($"AuthViewModel.LoginAsync called with email: {LoginEmail}");
        
        // Clear previous error
        LoginErrorMessage = null;
        
        // Basic validation
        if (string.IsNullOrWhiteSpace(LoginEmail))
        {
            LoginErrorMessage = "Please enter your email address.";
            Console.WriteLine("Login failed: Empty email");
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(LoginPassword))
        {
            LoginErrorMessage = "Please enter your password.";
            Console.WriteLine("Login failed: Empty password");
            return false;
        }
        
        if (LoginPassword.Length < 6)
        {
            LoginErrorMessage = "Password must be at least 6 characters long.";
            Console.WriteLine("Login failed: Password too short");
            return false;
        }

        IsLoggingIn = true;
        Console.WriteLine("Starting login process...");

        try
        {
            var result = await _authService.LoginAsync(LoginEmail, LoginPassword, RememberMe);
            
            if (result.Success)
            {
                IsAuthenticated = true;
                CurrentUserEmail = result.User?.Email ?? LoginEmail;
                LoginEmail = string.Empty;
                LoginPassword = string.Empty;
                AuthenticationStateChanged?.Invoke(true);
                return true;
            }
            else
            {
                LoginErrorMessage = result.Message;
                if (result.Errors.Any())
                {
                    LoginErrorMessage += " " + string.Join(", ", result.Errors);
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            LoginErrorMessage = $"An error occurred: {ex.Message}";
            return false;
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    public async Task<bool> RegisterAsync()
    {
        // Clear previous error
        RegisterErrorMessage = null;
        
        // Basic validation
        if (string.IsNullOrWhiteSpace(RegisterEmail))
        {
            RegisterErrorMessage = "Please enter your email address.";
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(RegisterPassword))
        {
            RegisterErrorMessage = "Please enter your password.";
            return false;
        }

        if (RegisterPassword != ConfirmPassword)
        {
            RegisterErrorMessage = "Passwords do not match.";
            return false;
        }

        if (RegisterPassword.Length < 6)
        {
            RegisterErrorMessage = "Password must be at least 6 characters long.";
            return false;
        }

        IsRegistering = true;

        try
        {
            var result = await _authService.RegisterAsync(RegisterEmail, RegisterPassword, ConfirmPassword);
            
            if (result.Success)
            {
                IsAuthenticated = true;
                CurrentUserEmail = result.User?.Email ?? RegisterEmail;
                RegisterEmail = string.Empty;
                RegisterPassword = string.Empty;
                ConfirmPassword = string.Empty;
                AuthenticationStateChanged?.Invoke(true);
                return true;
            }
            else
            {
                RegisterErrorMessage = result.Message;
                if (result.Errors.Any())
                {
                    RegisterErrorMessage += " " + string.Join(", ", result.Errors);
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            RegisterErrorMessage = $"An error occurred: {ex.Message}";
            return false;
        }
        finally
        {
            IsRegistering = false;
        }
    }

    public async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        IsAuthenticated = false;
        CurrentUserEmail = null;
        ClearLoginForm();
        ClearRegisterForm();
        AuthenticationStateChanged?.Invoke(false);
    }

    public async Task CheckAuthenticationStatusAsync()
    {
        IsAuthenticated = await _authService.IsAuthenticatedAsync();
        if (IsAuthenticated)
        {
            var user = await _authService.GetCurrentUserAsync();
            CurrentUserEmail = user?.Email;
        }
    }

    public void ClearLoginForm()
    {
        LoginEmail = string.Empty;
        LoginPassword = string.Empty;
        RememberMe = false;
        LoginErrorMessage = null;
    }

    public void ClearRegisterForm()
    {
        RegisterEmail = string.Empty;
        RegisterPassword = string.Empty;
        ConfirmPassword = string.Empty;
        RegisterErrorMessage = null;
    }

    public void ClearAllForms()
    {
        ClearLoginForm();
        ClearRegisterForm();
    }
}
