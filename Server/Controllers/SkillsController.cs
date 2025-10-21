using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly SkillSnapContext _context;

    public SkillsController(SkillSnapContext context)
    {
        _context = context;
    }

    // GET: api/Skills
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
    {
        try
        {
            var skills = await _context.Skills
                .Include(s => s.PortfolioUser)
                .ToListAsync();

            return Ok(skills);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving skills.", error = ex.Message });
        }
    }

    // GET: api/Skills/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        try
        {
            var skill = await _context.Skills
                .Include(s => s.PortfolioUser)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
            {
                return NotFound(new { message = $"Skill with ID {id} not found." });
            }

            return Ok(skill);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the skill.", error = ex.Message });
        }
    }

    // GET: api/Skills/search?name=react&level=expert&portfolioUserId=1
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Skill>>> SearchSkills(
        [FromQuery] string? name = null,
        [FromQuery] string? level = null,
        [FromQuery] int? portfolioUserId = null)
    {
        try
        {
            var query = _context.Skills
                .Include(s => s.PortfolioUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(s => s.Level.Equals(level, StringComparison.OrdinalIgnoreCase));
            }

            if (portfolioUserId.HasValue)
            {
                query = query.Where(s => s.PortfolioUserId == portfolioUserId.Value);
            }

            var skills = await query.ToListAsync();
            return Ok(skills);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while searching skills.", error = ex.Message });
        }
    }

    // GET: api/Skills/by-user/5
    [HttpGet("by-user/{portfolioUserId}")]
    public async Task<ActionResult<IEnumerable<Skill>>> GetSkillsByUser(int portfolioUserId)
    {
        try
        {
            var portfolioUser = await _context.PortfolioUsers.FindAsync(portfolioUserId);
            if (portfolioUser == null)
            {
                return NotFound(new { message = $"Portfolio user with ID {portfolioUserId} not found." });
            }

            var skills = await _context.Skills
                .Where(s => s.PortfolioUserId == portfolioUserId)
                .Include(s => s.PortfolioUser)
                .ToListAsync();

            return Ok(skills);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving skills by user.", error = ex.Message });
        }
    }

    // GET: api/Skills/by-level/expert
    [HttpGet("by-level/{level}")]
    public async Task<ActionResult<IEnumerable<Skill>>> GetSkillsByLevel(string level)
    {
        try
        {
            var skills = await _context.Skills
                .Where(s => s.Level.Equals(level, StringComparison.OrdinalIgnoreCase))
                .Include(s => s.PortfolioUser)
                .ToListAsync();

            return Ok(skills);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving skills by level.", error = ex.Message });
        }
    }

    // POST: api/Skills
    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(Skill skill)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(skill.Name))
            {
                return BadRequest(new { message = "Name is required." });
            }

            if (string.IsNullOrWhiteSpace(skill.Level))
            {
                return BadRequest(new { message = "Level is required." });
            }

            if (skill.PortfolioUserId <= 0)
            {
                return BadRequest(new { message = "Valid PortfolioUserId is required." });
            }

            // Validate skill level
            var validLevels = new[] { "Beginner", "Novice", "Intermediate", "Advanced", "Expert", "Master" };
            if (!validLevels.Contains(skill.Level, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = $"Invalid skill level. Valid levels are: {string.Join(", ", validLevels)}" });
            }

            // Verify that the portfolio user exists
            var portfolioUser = await _context.PortfolioUsers.FindAsync(skill.PortfolioUserId);
            if (portfolioUser == null)
            {
                return BadRequest(new { message = $"Portfolio user with ID {skill.PortfolioUserId} not found." });
            }

            // Check if the user already has this skill
            var existingSkill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Name.Equals(skill.Name, StringComparison.OrdinalIgnoreCase) 
                                        && s.PortfolioUserId == skill.PortfolioUserId);
            
            if (existingSkill != null)
            {
                return BadRequest(new { message = $"The user already has the skill '{skill.Name}'. Use PUT to update the skill level." });
            }

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            // Return the skill with the portfolio user included
            var createdSkill = await _context.Skills
                .Include(s => s.PortfolioUser)
                .FirstOrDefaultAsync(s => s.Id == skill.Id);

            return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, createdSkill);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the skill.", error = ex.Message });
        }
    }

    // PUT: api/Skills/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSkill(int id, Skill skill)
    {
        try
        {
            if (id != skill.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(skill.Name))
            {
                return BadRequest(new { message = "Name is required." });
            }

            if (string.IsNullOrWhiteSpace(skill.Level))
            {
                return BadRequest(new { message = "Level is required." });
            }

            if (skill.PortfolioUserId <= 0)
            {
                return BadRequest(new { message = "Valid PortfolioUserId is required." });
            }

            // Validate skill level
            var validLevels = new[] { "Beginner", "Novice", "Intermediate", "Advanced", "Expert", "Master" };
            if (!validLevels.Contains(skill.Level, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = $"Invalid skill level. Valid levels are: {string.Join(", ", validLevels)}" });
            }

            // Verify that the portfolio user exists
            var portfolioUser = await _context.PortfolioUsers.FindAsync(skill.PortfolioUserId);
            if (portfolioUser == null)
            {
                return BadRequest(new { message = $"Portfolio user with ID {skill.PortfolioUserId} not found." });
            }

            var existingSkill = await _context.Skills.FindAsync(id);
            if (existingSkill == null)
            {
                return NotFound(new { message = $"Skill with ID {id} not found." });
            }

            // Check if another skill with the same name exists for this user (excluding current skill)
            var duplicateSkill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Name.Equals(skill.Name, StringComparison.OrdinalIgnoreCase) 
                                        && s.PortfolioUserId == skill.PortfolioUserId
                                        && s.Id != id);
            
            if (duplicateSkill != null)
            {
                return BadRequest(new { message = $"The user already has another skill with the name '{skill.Name}'." });
            }

            // Update properties
            existingSkill.Name = skill.Name;
            existingSkill.Level = skill.Level;
            existingSkill.PortfolioUserId = skill.PortfolioUserId;

            _context.Entry(existingSkill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillExists(id))
                {
                    return NotFound(new { message = $"Skill with ID {id} not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the skill.", error = ex.Message });
        }
    }

    // DELETE: api/Skills/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        try
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound(new { message = $"Skill with ID {id} not found." });
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the skill.", error = ex.Message });
        }
    }

    // GET: api/Skills/statistics
    [HttpGet("statistics")]
    public async Task<ActionResult<object>> GetSkillStatistics()
    {
        try
        {
            var totalSkills = await _context.Skills.CountAsync();
            
            var skillsByLevel = await _context.Skills
                .GroupBy(s => s.Level)
                .Select(g => new { Level = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            var mostPopularSkills = await _context.Skills
                .GroupBy(s => s.Name)
                .Select(g => new { SkillName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            var skillsByUser = await _context.Skills
                .GroupBy(s => s.PortfolioUserId)
                .Select(g => new { PortfolioUserId = g.Key, Count = g.Count() })
                .ToListAsync();

            var averageSkillsPerUser = skillsByUser.Any() ? skillsByUser.Average(x => x.Count) : 0;

            return Ok(new
            {
                TotalSkills = totalSkills,
                TotalUsers = skillsByUser.Count,
                AverageSkillsPerUser = Math.Round(averageSkillsPerUser, 2),
                SkillsByLevel = skillsByLevel,
                MostPopularSkills = mostPopularSkills,
                SkillsByUser = skillsByUser
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving skill statistics.", error = ex.Message });
        }
    }

    // GET: api/Skills/levels
    [HttpGet("levels")]
    public ActionResult<IEnumerable<string>> GetSkillLevels()
    {
        var levels = new[] { "Beginner", "Novice", "Intermediate", "Advanced", "Expert", "Master" };
        return Ok(levels);
    }

    private bool SkillExists(int id)
    {
        return _context.Skills.Any(e => e.Id == id);
    }
}
