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
            Console.WriteLine($"[DEBUG] SkillViewModel.CreateSkillAsync called with skill: {skill.Name}, {skill.Level}, PortfolioUserId={skill.PortfolioUserId}");
            try
            {
                Console.WriteLine("[DEBUG] Calling _skillService.CreateSkillAsync...");
                var createdSkill = await _skillService.CreateSkillAsync(skill);
                Console.WriteLine($"[DEBUG] _skillService.CreateSkillAsync returned: {createdSkill != null}");
                
                if (createdSkill != null)
                {
                    Console.WriteLine("[DEBUG] Skill created successfully, refreshing skills list...");
                    // Refresh the skills list
                    await LoadSkillsInternalAsync();
                    Console.WriteLine("[DEBUG] Skills list refreshed");
                }
                else
                {
                    Console.WriteLine("[DEBUG] Skill creation returned null");
                }
                return createdSkill;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception in CreateSkillAsync: {ex.Message}");
                Console.WriteLine($"[DEBUG] Exception type: {ex.GetType().Name}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
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