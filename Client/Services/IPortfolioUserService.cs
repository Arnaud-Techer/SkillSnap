using Client.Models;
using Shared.Models;

namespace Client.Services;

public interface IPortfolioUserService
{
    // GET operations
    Task<IEnumerable<PortfolioUserSummary>> GetPortfolioUsersAsync();
    Task<PortfolioUser?> GetPortfolioUserAsync(int id);
    Task<IEnumerable<PortfolioUserSummary>> SearchPortfolioUsersAsync(string? name = null);
    Task<object?> GetPortfolioUserStatisticsAsync(int id);
    Task<IEnumerable<Project>> GetPortfolioUserProjectsAsync(int id);
    Task<IEnumerable<Skill>> GetPortfolioUserSkillsAsync(int id);

    // POST operations
    Task<PortfolioUser?> CreatePortfolioUserAsync(PortfolioUser portfolioUser);

    // PUT operations
    Task<bool> UpdatePortfolioUserAsync(int id, PortfolioUser portfolioUser);

    // DELETE operations
    Task<bool> DeletePortfolioUserAsync(int id);

    // Utility methods
    Task<bool> PortfolioUserExistsAsync(int id);
}
