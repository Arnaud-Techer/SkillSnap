using System.Net.Http.Json;
using System.Text.Json;
using Shared.Models;

namespace Client.Services;

public class SkillService : ISkillService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public SkillService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // GET operations
    public async Task<IEnumerable<Skill>> GetSkillsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Skill>>("api/Skills", _jsonOptions);
            return response ?? Enumerable.Empty<Skill>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return Enumerable.Empty<Skill>();
        }
    }

    public async Task<Skill?> GetSkillAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Skill>($"api/Skills/{id}", _jsonOptions);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<Skill>> GetSkillsByUserAsync(int portfolioUserId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Skill>>($"api/Skills/by-user/{portfolioUserId}", _jsonOptions);
            return response ?? Enumerable.Empty<Skill>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return Enumerable.Empty<Skill>();
        }
    }

    public async Task<IEnumerable<Skill>> SearchSkillsAsync(string? name = null, string? level = null, int? portfolioUserId = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(name))
                queryParams.Add($"name={Uri.EscapeDataString(name)}");
            if (!string.IsNullOrEmpty(level))
                queryParams.Add($"level={Uri.EscapeDataString(level)}");
            if (portfolioUserId.HasValue)
                queryParams.Add($"portfolioUserId={portfolioUserId.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Skill>>($"api/Skills/search{queryString}", _jsonOptions);
            return response ?? Enumerable.Empty<Skill>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return Enumerable.Empty<Skill>();
        }
    }

    public async Task<IEnumerable<Skill>> GetSkillsByLevelAsync(string level)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Skill>>($"api/Skills/by-level/{Uri.EscapeDataString(level)}", _jsonOptions);
            return response ?? Enumerable.Empty<Skill>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return Enumerable.Empty<Skill>();
        }
    }

    public async Task<object?> GetSkillStatisticsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<object>("api/Skills/statistics", _jsonOptions);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<string>> GetSkillLevelsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<string>>("api/Skills/levels", _jsonOptions);
            return response ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }

    // POST operations
    public async Task<Skill?> CreateSkillAsync(Skill skill)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Skills", skill, _jsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var createdSkill = await response.Content.ReadFromJsonAsync<Skill>(_jsonOptions);
                return createdSkill;
            }
            else
            {
                Console.WriteLine($"Failed to create skill. Status: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    // PUT operations
    public async Task<bool> UpdateSkillAsync(int id, Skill skill)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Skills/{id}", skill, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    // DELETE operations
    public async Task<bool> DeleteSkillAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Skills/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
}
