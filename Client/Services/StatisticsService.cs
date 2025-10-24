using System.Text.Json;

namespace Client.Services;

public class StatisticsService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public StatisticsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<StatisticsData?> GetStatisticsAsync()
    {
        try
        {
            // Use the dedicated portfolio statistics endpoint
            var response = await _httpClient.GetStringAsync("api/PortfolioUsers/statistics");
            var stats = JsonSerializer.Deserialize<PortfolioStatistics>(response, _jsonOptions);

            if (stats == null)
            {
                return null;
            }

            return new StatisticsData
            {
                TotalPortfolios = stats.TotalPortfolioUsers,
                TotalProjects = stats.TotalProjects,
                TotalSkills = stats.TotalSkills
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public class StatisticsData
{
    public int TotalPortfolios { get; set; }
    public int TotalProjects { get; set; }
    public int TotalSkills { get; set; }
}

public class PortfolioStatistics
{
    public int TotalPortfolioUsers { get; set; }
    public int TotalProjects { get; set; }
    public int TotalSkills { get; set; }
}
