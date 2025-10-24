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
        catch (Exception)
        {
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
        catch (Exception)
        {
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
        catch (Exception)
        {
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
        catch (Exception)
        {
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
        catch (Exception)
        {
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
        catch (Exception)
        {
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
        catch (Exception)
        {
            return Enumerable.Empty<string>();
        }
    }

    // POST operations
    public async Task<Skill?> CreateSkillAsync(Skill skill)
    {
        Console.WriteLine($"[DEBUG] SkillService.CreateSkillAsync called with skill: {skill.Name}, {skill.Level}, PortfolioUserId={skill.PortfolioUserId}");
        try
        {
            Console.WriteLine("[DEBUG] Making HTTP POST request to api/Skills...");
            var response = await _httpClient.PostAsJsonAsync("api/Skills", skill, _jsonOptions);
            Console.WriteLine($"[DEBUG] HTTP response received. Status: {response.StatusCode}, IsSuccessStatusCode: {response.IsSuccessStatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("[DEBUG] Response is successful, reading content...");
                var createdSkill = await response.Content.ReadFromJsonAsync<Skill>(_jsonOptions);
                Console.WriteLine($"[DEBUG] Created skill from response: {createdSkill?.Name}, {createdSkill?.Level}, PortfolioUserId={createdSkill?.PortfolioUserId}");
                return createdSkill;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("[DEBUG] Unauthorized response received");
                throw new UnauthorizedAccessException("You must be signed in to create skills.");
            }
            else
            {
                Console.WriteLine($"[DEBUG] Non-success response: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Error content: {errorContent}");
                return null;
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("[DEBUG] UnauthorizedAccessException caught and re-thrown");
            throw; // Re-throw to be handled by the UI
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Exception in CreateSkillAsync: {ex.Message}");
            Console.WriteLine($"[DEBUG] Exception type: {ex.GetType().Name}");
            Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
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
        catch (Exception)
        {
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
        catch (Exception)
        {
            return false;
        }
    }
}
