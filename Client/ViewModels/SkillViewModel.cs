using Shared.Models;
using Client.Services;

namespace Client.ViewModels
{
    public class SkillViewModel 
    {
        private readonly ISkillService _skillService;
        
        public IEnumerable<Skill> Skills { get; private set; } = Enumerable.Empty<Skill>();
        public bool IsLoading { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? SearchName { get; set; }
        public string? SearchLevel { get; set; }
        public int? SearchPortfolioUserId { get; set; }

        public SkillViewModel(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task LoadSkillsAsync()
        {
            await LoadSkillsInternalAsync();
        }

        public async Task LoadSkillsByUserAsync(int portfolioUserId)
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                Skills = await _skillService.GetSkillsByUserAsync(portfolioUserId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load skills: {ex.Message}";
                Skills = Enumerable.Empty<Skill>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SearchSkillsAsync()
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                Skills = await _skillService.SearchSkillsAsync(SearchName, SearchLevel, SearchPortfolioUserId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to search skills: {ex.Message}";
                Skills = Enumerable.Empty<Skill>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadSkillsByLevelAsync(string level)
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                Skills = await _skillService.GetSkillsByLevelAsync(level);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load skills by level: {ex.Message}";
                Skills = Enumerable.Empty<Skill>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<Skill?> CreateSkillAsync(Skill skill)
        {
            try
            {
                var createdSkill = await _skillService.CreateSkillAsync(skill);
                
                if (createdSkill != null)
                {
                    // Refresh the skills list
                    await LoadSkillsInternalAsync();
                }
                return createdSkill;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to create skill: {ex.Message}";
                return null;
            }
        }

        public async Task<bool> UpdateSkillAsync(int id, Skill skill)
        {
            try
            {
                var success = await _skillService.UpdateSkillAsync(id, skill);
                if (success)
                {
                    // Refresh the skills list
                    await LoadSkillsInternalAsync();
                }
                return success;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update skill: {ex.Message}";
                return false;
            }
        }

        public async Task<bool> DeleteSkillAsync(int id)
        {
            try
            {
                var success = await _skillService.DeleteSkillAsync(id);
                if (success)
                {
                    // Refresh the skills list
                    await LoadSkillsInternalAsync();
                }
                return success;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to delete skill: {ex.Message}";
                return false;
            }
        }

        public void ClearSearch()
        {
            SearchName = null;
            SearchLevel = null;
            SearchPortfolioUserId = null;
        }

        public void ClearError()
        {
            ErrorMessage = null;
        }

        private async Task LoadSkillsInternalAsync()
        {
            IsLoading = true;
            ErrorMessage = null;
            
            try
            {
                Skills = await _skillService.GetSkillsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load skills: {ex.Message}";
                Skills = Enumerable.Empty<Skill>();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}