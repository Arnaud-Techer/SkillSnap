using System.Net.Http.Json;
using System.Text.Json;
using Shared.Models;

namespace Client.Services;

public class ProjectService : IProjectService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // GET operations
    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Project>>("api/Projects", _jsonOptions);
            return response ?? Enumerable.Empty<Project>();
        }
        catch (Exception)
        {
            return Enumerable.Empty<Project>();
        }
    }

    public async Task<Project?> GetProjectAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Project>($"api/Projects/{id}", _jsonOptions);
            return response;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserAsync(int portfolioUserId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Project>>($"api/Projects/by-user/{portfolioUserId}", _jsonOptions);
            return response ?? Enumerable.Empty<Project>();
        }
        catch (Exception)
        {
            return Enumerable.Empty<Project>();
        }
    }

    public async Task<IEnumerable<Project>> SearchProjectsAsync(string? title = null, int? portfolioUserId = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(title))
                queryParams.Add($"title={Uri.EscapeDataString(title)}");
            if (portfolioUserId.HasValue)
                queryParams.Add($"portfolioUserId={portfolioUserId.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Project>>($"api/Projects/search{queryString}", _jsonOptions);
            return response ?? Enumerable.Empty<Project>();
        }
        catch (Exception)
        {
            return Enumerable.Empty<Project>();
        }
    }

    public async Task<object?> GetProjectStatisticsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<object>("api/Projects/statistics", _jsonOptions);
            return response;
        }
        catch (Exception)
        {
            return null;
        }
    }

    // POST operations
    public async Task<Project?> CreateProjectAsync(Project project)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Projects", project, _jsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var createdProject = await response.Content.ReadFromJsonAsync<Project>(_jsonOptions);
                return createdProject;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("You must be signed in to create projects.");
            }
            else
            {
                return null;
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw to be handled by the UI
        }
        catch (Exception)
        {
            return null;
        }
    }

    // PUT operations
    public async Task<bool> UpdateProjectAsync(int id, Project project)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Projects/{id}", project, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // DELETE operations
    public async Task<bool> DeleteProjectAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Projects/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
