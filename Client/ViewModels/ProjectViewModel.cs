using Shared.Models;
using Client.Services;

namespace Client.ViewModels
{
    public class ProjectViewModel
    {
        private readonly IProjectService _projectService;
        
        public IEnumerable<Project> Projects { get; private set; } = Enumerable.Empty<Project>();
        public bool IsLoading { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? SearchTitle { get; set; }
        public int? SearchPortfolioUserId { get; set; }

        public ProjectViewModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task LoadProjectsAsync()
        {
            await LoadProjectsInternalAsync();
        }

        public async Task LoadProjectsByUserAsync(int portfolioUserId)
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                Projects = await _projectService.GetProjectsByUserAsync(portfolioUserId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load projects: {ex.Message}";
                Projects = Enumerable.Empty<Project>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SearchProjectsAsync()
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                Projects = await _projectService.SearchProjectsAsync(SearchTitle, SearchPortfolioUserId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to search projects: {ex.Message}";
                Projects = Enumerable.Empty<Project>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<Project?> GetProjectAsync(int id)
        {
            try
            {
                return await _projectService.GetProjectAsync(id);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to get project: {ex.Message}";
                return null;
            }
        }

        public async Task<Project?> CreateProjectAsync(Project project)
        {
            try
            {
                var createdProject = await _projectService.CreateProjectAsync(project);
                if (createdProject != null)
                {
                    // Refresh the projects list
                    await LoadProjectsInternalAsync();
                }
                return createdProject;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to create project: {ex.Message}";
                return null;
            }
        }

        public async Task<bool> UpdateProjectAsync(int id, Project project)
        {
            try
            {
                var success = await _projectService.UpdateProjectAsync(id, project);
                if (success)
                {
                    // Refresh the projects list
                    await LoadProjectsInternalAsync();
                }
                return success;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update project: {ex.Message}";
                return false;
            }
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            try
            {
                var success = await _projectService.DeleteProjectAsync(id);
                if (success)
                {
                    // Refresh the projects list
                    await LoadProjectsInternalAsync();
                }
                return success;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete project: {ex.Message}";
                return false;
            }
        }

        public void ClearSearch()
        {
            SearchTitle = null;
            SearchPortfolioUserId = null;
        }

        public void ClearError()
        {
            ErrorMessage = null;
        }

        private async Task LoadProjectsInternalAsync()
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                Projects = await _projectService.GetProjectsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load projects: {ex.Message}";
                Projects = Enumerable.Empty<Project>();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
