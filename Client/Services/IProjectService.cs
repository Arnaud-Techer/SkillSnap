using Shared.Models;

namespace Client.Services;

public interface IProjectService
{
    // GET operations
    Task<IEnumerable<Project>> GetProjectsAsync();
    Task<Project?> GetProjectAsync(int id);
    Task<IEnumerable<Project>> GetProjectsByUserAsync(int portfolioUserId);
    Task<IEnumerable<Project>> SearchProjectsAsync(string? title = null, int? portfolioUserId = null);
    Task<object?> GetProjectStatisticsAsync();

    // POST operations
    Task<Project?> CreateProjectAsync(Project project);

    // PUT operations
    Task<bool> UpdateProjectAsync(int id, Project project);

    // DELETE operations
    Task<bool> DeleteProjectAsync(int id);
}
