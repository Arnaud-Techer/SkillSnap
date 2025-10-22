using System.Net.Http.Json;
using System.Text.Json;
using Client.Models;
using Shared.Models;

namespace Client.Services;

public class PortfolioUserService : IPortfolioUserService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public PortfolioUserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // GET operations
    public async Task<IEnumerable<PortfolioUserSummary>> GetPortfolioUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<object>>("api/PortfolioUsers", _jsonOptions);
            if (response == null)
                return Enumerable.Empty<PortfolioUserSummary>();

            // Convert anonymous objects to PortfolioUserSummary
            var portfolioUsers = new List<PortfolioUserSummary>();
            foreach (var item in response)
            {
                var jsonElement = (System.Text.Json.JsonElement)item;
                portfolioUsers.Add(new PortfolioUserSummary
                {
                    Id = jsonElement.GetProperty("id").GetInt32(),
                    Name = jsonElement.GetProperty("name").GetString() ?? string.Empty,
                    Bio = jsonElement.GetProperty("bio").GetString() ?? string.Empty,
                    ProfileImageUrl = jsonElement.TryGetProperty("profileImageUrl", out var imageUrl) ? imageUrl.GetString() : null,
                    ProjectCount = jsonElement.GetProperty("projectCount").GetInt32(),
                    SkillCount = jsonElement.GetProperty("skillCount").GetInt32()
                });
            }
            return portfolioUsers;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            throw new Exception($"Failed to connect to the server: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            throw new Exception($"Request timeout: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw new Exception($"Failed to load portfolio users: {ex.Message}", ex);
        }
    }

    public async Task<PortfolioUser?> GetPortfolioUserAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<PortfolioUser>($"api/PortfolioUsers/{id}", _jsonOptions);
            return response;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<PortfolioUserSummary>> SearchPortfolioUsersAsync(string? name = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(name))
            {
                queryParams.Add($"name={Uri.EscapeDataString(name)}");
            }

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<object>>($"api/PortfolioUsers/search{queryString}", _jsonOptions);
            if (response == null)
                return Enumerable.Empty<PortfolioUserSummary>();

            // Convert anonymous objects to PortfolioUserSummary
            var portfolioUsers = new List<PortfolioUserSummary>();
            foreach (var item in response)
            {
                var jsonElement = (System.Text.Json.JsonElement)item;
                portfolioUsers.Add(new PortfolioUserSummary
                {
                    Id = jsonElement.GetProperty("id").GetInt32(),
                    Name = jsonElement.GetProperty("name").GetString() ?? string.Empty,
                    Bio = jsonElement.GetProperty("bio").GetString() ?? string.Empty,
                    ProfileImageUrl = jsonElement.TryGetProperty("profileImageUrl", out var imageUrl) ? imageUrl.GetString() : null,
                    ProjectCount = jsonElement.GetProperty("projectCount").GetInt32(),
                    SkillCount = jsonElement.GetProperty("skillCount").GetInt32()
                });
            }
            return portfolioUsers;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            throw new Exception($"Failed to connect to the server: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            throw new Exception($"Request timeout: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw new Exception($"Failed to search portfolio users: {ex.Message}", ex);
        }
    }

    public async Task<object?> GetPortfolioUserStatisticsAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<object>($"api/PortfolioUsers/{id}/statistics", _jsonOptions);
            return response;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<Project>> GetPortfolioUserProjectsAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Project>>($"api/PortfolioUsers/{id}/projects", _jsonOptions);
            return response ?? Enumerable.Empty<Project>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            return Enumerable.Empty<Project>();
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            return Enumerable.Empty<Project>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return Enumerable.Empty<Project>();
        }
    }

    public async Task<IEnumerable<Skill>> GetPortfolioUserSkillsAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Skill>>($"api/PortfolioUsers/{id}/skills", _jsonOptions);
            return response ?? Enumerable.Empty<Skill>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            return Enumerable.Empty<Skill>();
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return Enumerable.Empty<Skill>();
        }
    }

    // POST operations
    public async Task<PortfolioUser?> CreatePortfolioUserAsync(PortfolioUser portfolioUser)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/PortfolioUsers", portfolioUser, _jsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var createdUser = await response.Content.ReadFromJsonAsync<PortfolioUser>(_jsonOptions);
                return createdUser;
            }
            else
            {
                Console.WriteLine($"Failed to create portfolio user. Status: {response.StatusCode}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    // PUT operations
    public async Task<bool> UpdatePortfolioUserAsync(int id, PortfolioUser portfolioUser)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/PortfolioUsers/{id}", portfolioUser, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    // DELETE operations
    public async Task<bool> DeletePortfolioUserAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/PortfolioUsers/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    // Utility methods
    public async Task<bool> PortfolioUserExistsAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/PortfolioUsers/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP error occurred: {ex.Message}");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Request was cancelled: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
}
