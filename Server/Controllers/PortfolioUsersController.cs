using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioUsersController : ControllerBase
{
    private readonly SkillSnapContext _context;

    public PortfolioUsersController(SkillSnapContext context)
    {
        _context = context;
    }

    // GET: api/PortfolioUsers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetPortfolioUsers()
    {
        try
        {
            var portfolioUsers = await _context.PortfolioUsers
                .Select(u => new
                {
                    Id = u.Id,
                    Name = u.Name,
                    Bio = u.Bio,
                    ProfileImageUrl = u.ProfileImageUrl,
                    ProjectCount = u.Projects.Count(), // EF Core translates this to SQL COUNT
                    SkillCount = u.Skills.Count()      // EF Core translates this to SQL COUNT
                })
                .ToListAsync();

            return Ok(portfolioUsers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving portfolio users.", error = ex.Message });
        }
    }

    // GET: api/PortfolioUsers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PortfolioUser>> GetPortfolioUser(int id)
    {
        try
        {
            var portfolioUser = await _context.PortfolioUsers
                .Include(p => p.Projects)
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (portfolioUser == null)
            {
                return NotFound(new { message = $"Portfolio user with ID {id} not found." });
            }

            return Ok(portfolioUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the portfolio user.", error = ex.Message });
        }
    }

    // GET: api/PortfolioUsers/search?name=john
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<object>>> SearchPortfolioUsers([FromQuery] string? name = null)
    {
        try
        {
            var query = _context.PortfolioUsers
                .Select(u => new
                {
                    Id = u.Id,
                    Name = u.Name,
                    Bio = u.Bio,
                    ProfileImageUrl = u.ProfileImageUrl,
                    ProjectCount = u.Projects.Count(),
                    SkillCount = u.Skills.Count()
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }

            var portfolioUsers = await query.ToListAsync();
            return Ok(portfolioUsers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while searching portfolio users.", error = ex.Message });
        }
    }

    // POST: api/PortfolioUsers
    [HttpPost]
    public async Task<ActionResult<PortfolioUser>> CreatePortfolioUser(PortfolioUser portfolioUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(portfolioUser.Name))
            {
                return BadRequest(new { message = "Name is required." });
            }

            if (string.IsNullOrWhiteSpace(portfolioUser.Bio))
            {
                return BadRequest(new { message = "Bio is required." });
            }

            _context.PortfolioUsers.Add(portfolioUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPortfolioUser), new { id = portfolioUser.Id }, portfolioUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the portfolio user.", error = ex.Message });
        }
    }

    // PUT: api/PortfolioUsers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePortfolioUser(int id, PortfolioUser portfolioUser)
    {
        try
        {
            if (id != portfolioUser.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(portfolioUser.Name))
            {
                return BadRequest(new { message = "Name is required." });
            }

            if (string.IsNullOrWhiteSpace(portfolioUser.Bio))
            {
                return BadRequest(new { message = "Bio is required." });
            }

            var existingUser = await _context.PortfolioUsers.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = $"Portfolio user with ID {id} not found." });
            }

            // Update properties
            existingUser.Name = portfolioUser.Name;
            existingUser.Bio = portfolioUser.Bio;
            existingUser.ProfileImageUrl = portfolioUser.ProfileImageUrl;

            _context.Entry(existingUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortfolioUserExists(id))
                {
                    return NotFound(new { message = $"Portfolio user with ID {id} not found." });
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
            return StatusCode(500, new { message = "An error occurred while updating the portfolio user.", error = ex.Message });
        }
    }

    // DELETE: api/PortfolioUsers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePortfolioUser(int id)
    {
        try
        {
            var portfolioUser = await _context.PortfolioUsers.FindAsync(id);
            if (portfolioUser == null)
            {
                return NotFound(new { message = $"Portfolio user with ID {id} not found." });
            }

            _context.PortfolioUsers.Remove(portfolioUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the portfolio user.", error = ex.Message });
        }
    }

    // GET: api/PortfolioUsers/5/projects
    [HttpGet("{id}/projects")]
    public async Task<ActionResult<IEnumerable<Project>>> GetPortfolioUserProjects(int id)
    {
        try
        {
            var portfolioUser = await _context.PortfolioUsers.FindAsync(id);
            if (portfolioUser == null)
            {
                return NotFound(new { message = $"Portfolio user with ID {id} not found." });
            }

            var projects = await _context.Projects
                .Where(p => p.PortfolioUserId == id)
                .ToListAsync();

            return Ok(projects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving projects.", error = ex.Message });
        }
    }

    // GET: api/PortfolioUsers/5/skills
    [HttpGet("{id}/skills")]
    public async Task<ActionResult<IEnumerable<Skill>>> GetPortfolioUserSkills(int id)
    {
        try
        {
            var portfolioUser = await _context.PortfolioUsers.FindAsync(id);
            if (portfolioUser == null)
            {
                return NotFound(new { message = $"Portfolio user with ID {id} not found." });
            }

            var skills = await _context.Skills
                .Where(s => s.PortfolioUserId == id)
                .ToListAsync();

            return Ok(skills);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving skills.", error = ex.Message });
        }
    }

    // GET: api/PortfolioUsers/5/statistics
    [HttpGet("{id}/statistics")]
    public async Task<ActionResult<object>> GetPortfolioUserStatistics(int id)
    {
        try
        {
            var portfolioUser = await _context.PortfolioUsers.FindAsync(id);
            if (portfolioUser == null)
            {
                return NotFound(new { message = $"Portfolio user with ID {id} not found." });
            }

            var statistics = new
            {
                ProjectCount = await _context.Projects
                    .CountAsync(p => p.PortfolioUserId == id),
                SkillCount = await _context.Skills
                    .CountAsync(s => s.PortfolioUserId == id)
            };
            
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving statistics.", error = ex.Message });
        }
    }

    // GET: api/PortfolioUsers/statistics
    [HttpGet("statistics")]
    public async Task<ActionResult<object>> GetPortfolioStatistics()
    {
        try
        {
            var totalPortfolioUsers = await _context.PortfolioUsers.CountAsync();
            var totalProjects = await _context.Projects.CountAsync();
            var totalSkills = await _context.Skills.CountAsync();

            var statistics = new
            {
                TotalPortfolioUsers = totalPortfolioUsers,
                TotalProjects = totalProjects,
                TotalSkills = totalSkills
            };

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving portfolio statistics.", error = ex.Message });
        }
    }

    private bool PortfolioUserExists(int id)
    {
        return _context.PortfolioUsers.Any(e => e.Id == id);
    }
}
