using Shared.Models;
using Client.Services;
using Client.Models;

namespace Client.ViewModels
{
    public class PortfolioUserViewModel
    {
        private readonly IPortfolioUserService _portfolioUserService;
        private readonly ISkillService _skillService;
        private readonly IProjectService _projectService;
        
        public IEnumerable<PortfolioUserSummary> PortfolioUsers { get; private set; } = Enumerable.Empty<PortfolioUserSummary>();
        public PortfolioUser? SelectedUser { get; private set; }
        public IEnumerable<Skill> UserSkills { get; private set; } = Enumerable.Empty<Skill>();
        public IEnumerable<Project> UserProjects { get; private set; } = Enumerable.Empty<Project>();
        public object? UserStatistics { get; private set; }
        
        public bool IsLoading { get; private set; }
        public bool IsLoadingSkills { get; private set; }
        public bool IsLoadingProjects { get; private set; }
        public bool IsLoadingStatistics { get; private set; }
        
        public string? ErrorMessage { get; private set; }
        public string? SearchName { get; set; }

        public PortfolioUserViewModel(
            IPortfolioUserService portfolioUserService,
            ISkillService skillService,
            IProjectService projectService)
        {
            _portfolioUserService = portfolioUserService;
            _skillService = skillService;
            _projectService = projectService;
        }

        public async Task LoadPortfolioUsersAsync()
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                PortfolioUsers = await _portfolioUserService.GetPortfolioUsersAsync();
                // Clear any previous error message on successful load
                ErrorMessage = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load portfolio users: {ex.Message}";
                PortfolioUsers = Enumerable.Empty<PortfolioUserSummary>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SearchPortfolioUsersAsync()
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                PortfolioUsers = await _portfolioUserService.SearchPortfolioUsersAsync(SearchName);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to search portfolio users: {ex.Message}";
                PortfolioUsers = Enumerable.Empty<PortfolioUserSummary>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<PortfolioUser?> LoadPortfolioUserAsync(int id)
        {
            try
            {
                SelectedUser = await _portfolioUserService.GetPortfolioUserAsync(id);
                return SelectedUser;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load portfolio user: {ex.Message}";
                SelectedUser = null;
                return null;
            }
        }

        public async Task LoadUserSkillsAsync(int userId)
        {
            IsLoadingSkills = true;
            ErrorMessage = null;
            
            try
            {
                UserSkills = await _skillService.GetSkillsByUserAsync(userId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load user skills: {ex.Message}";
                UserSkills = Enumerable.Empty<Skill>();
            }
            finally
            {
                IsLoadingSkills = false;
            }
        }

        public async Task LoadUserProjectsAsync(int userId)
        {
            IsLoadingProjects = true;
            ErrorMessage = null;
            
            try
            {
                UserProjects = await _projectService.GetProjectsByUserAsync(userId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load user projects: {ex.Message}";
                UserProjects = Enumerable.Empty<Project>();
            }
            finally
            {
                IsLoadingProjects = false;
            }
        }

        public async Task LoadUserStatisticsAsync(int userId)
        {
            IsLoadingStatistics = true;
            ErrorMessage = null;
            
            try
            {
                UserStatistics = await _portfolioUserService.GetPortfolioUserStatisticsAsync(userId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load user statistics: {ex.Message}";
                UserStatistics = null;
            }
            finally
            {
                IsLoadingStatistics = false;
            }
        }

        public async Task<PortfolioUser?> CreatePortfolioUserAsync(PortfolioUser portfolioUser)
        {
            try
            {
                var createdUser = await _portfolioUserService.CreatePortfolioUserAsync(portfolioUser);
                if (createdUser != null)
                {
                    // Refresh the portfolio users list
                    await LoadPortfolioUsersAsync();
                }
                return createdUser;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to create portfolio user: {ex.Message}";
                return null;
            }
        }

        public async Task<bool> UpdatePortfolioUserAsync(int id, PortfolioUser portfolioUser)
        {
            try
            {
                var success = await _portfolioUserService.UpdatePortfolioUserAsync(id, portfolioUser);
                if (success)
                {
                    // Refresh the portfolio users list
                    await LoadPortfolioUsersAsync();
                    
                    // Update selected user if it's the same one
                    if (SelectedUser?.Id == id)
                    {
                        SelectedUser = await _portfolioUserService.GetPortfolioUserAsync(id);
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update portfolio user: {ex.Message}";
                return false;
            }
        }

        public async Task<bool> DeletePortfolioUserAsync(int id)
        {
            try
            {
                var success = await _portfolioUserService.DeletePortfolioUserAsync(id);
                if (success)
                {
                    // Refresh the portfolio users list
                    await LoadPortfolioUsersAsync();
                    
                    // Clear selected user if it's the deleted one
                    if (SelectedUser?.Id == id)
                    {
                        SelectedUser = null;
                        UserSkills = Enumerable.Empty<Skill>();
                        UserProjects = Enumerable.Empty<Project>();
                        UserStatistics = null;
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete portfolio user: {ex.Message}";
                return false;
            }
        }

        public void SelectUser(PortfolioUserSummary userSummary)
        {
            SelectedUser = new PortfolioUser
            {
                Id = userSummary.Id,
                Name = userSummary.Name,
                Bio = userSummary.Bio,
                ProfileImageUrl = userSummary.ProfileImageUrl
            };
            
            // Clear previous user data
            UserSkills = Enumerable.Empty<Skill>();
            UserProjects = Enumerable.Empty<Project>();
            UserStatistics = null;
        }

        public void ClearSelection()
        {
            SelectedUser = null;
            UserSkills = Enumerable.Empty<Skill>();
            UserProjects = Enumerable.Empty<Project>();
            UserStatistics = null;
        }

        public void ClearSearch()
        {
            SearchName = null;
        }

        public void ClearError()
        {
            ErrorMessage = null;
        }

        public void ClearUserData()
        {
            UserSkills = Enumerable.Empty<Skill>();
            UserProjects = Enumerable.Empty<Project>();
            UserStatistics = null;
        }
    }
}
