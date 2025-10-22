using Shared.Models;

namespace Client.Services;

public interface ISkillService
{
    // GET operations
    Task<IEnumerable<Skill>> GetSkillsAsync();
    Task<Skill?> GetSkillAsync(int id);
    Task<IEnumerable<Skill>> GetSkillsByUserAsync(int portfolioUserId);
    Task<IEnumerable<Skill>> SearchSkillsAsync(string? name = null, string? level = null, int? portfolioUserId = null);
    Task<IEnumerable<Skill>> GetSkillsByLevelAsync(string level);
    Task<object?> GetSkillStatisticsAsync();
    Task<IEnumerable<string>> GetSkillLevelsAsync();

    // POST operations
    Task<Skill?> CreateSkillAsync(Skill skill);

    // PUT operations
    Task<bool> UpdateSkillAsync(int id, Skill skill);

    // DELETE operations
    Task<bool> DeleteSkillAsync(int id);
}
