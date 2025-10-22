using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly SkillSnapContext _context;
    private readonly IMemoryCache _cache;

    public ProjectsController(SkillSnapContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: api/Projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        try
        {
            const string cacheKey = "all_projects";
            
            if (_cache.TryGetValue(cacheKey, out List<Project>? cachedProjects))
            {
                return Ok(cachedProjects);
            }

            var projects = await _context.Projects
                .Include(p => p.PortfolioUser)
                .ToListAsync();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            _cache.Set(cacheKey, projects, cacheOptions);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving projects.", error = ex.Message });
        }
    }

    // GET: api/Projects/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        try
        {
            var project = await _context.Projects
                .Include(p => p.PortfolioUser)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound(new { message = $"Project with ID {id} not found." });
            }

            return Ok(project);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving the project.", error = ex.Message });
        }
    }

    // GET: api/Projects/search?title=react&portfolioUserId=1
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Project>>> SearchProjects(
        [FromQuery] string? title = null,
        [FromQuery] int? portfolioUserId = null)
    {
        try
        {
            var query = _context.Projects
                .Include(p => p.PortfolioUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }

            if (portfolioUserId.HasValue)
            {
                query = query.Where(p => p.PortfolioUserId == portfolioUserId.Value);
            }

            var projects = await query.ToListAsync();
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while searching projects.", error = ex.Message });
        }
    }

    // GET: api/Projects/by-user/5
    [HttpGet("by-user/{portfolioUserId}")]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByUser(int portfolioUserId)
    {
        try
        {
            string cacheKey = $"projects_user_{portfolioUserId}";
            
            if (_cache.TryGetValue(cacheKey, out List<Project>? cachedProjects))
            {
                return Ok(cachedProjects);
            }

            var portfolioUser = await _context.PortfolioUsers.FindAsync(portfolioUserId);
            if (portfolioUser == null)
            {
                return NotFound(new { message = $"Portfolio user with ID {portfolioUserId} not found." });
            }

            var projects = await _context.Projects
                .Where(p => p.PortfolioUserId == portfolioUserId)
                .Include(p => p.PortfolioUser)
                .ToListAsync();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            _cache.Set(cacheKey, projects, cacheOptions);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving projects by user.", error = ex.Message });
        }
    }

    // POST: api/Projects
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(Project project)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(project.Title))
            {
                return BadRequest(new { message = "Title is required." });
            }

            if (string.IsNullOrWhiteSpace(project.Description))
            {
                return BadRequest(new { message = "Description is required." });
            }

            if (project.PortfolioUserId <= 0)
            {
                return BadRequest(new { message = "Valid PortfolioUserId is required." });
            }

            // Verify that the portfolio user exists
            var portfolioUser = await _context.PortfolioUsers.FindAsync(project.PortfolioUserId);
            if (portfolioUser == null)
            {
                return BadRequest(new { message = $"Portfolio user with ID {project.PortfolioUserId} not found." });
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Invalidate caches for this user
            InvalidateUserCaches(project.PortfolioUserId);

            // Return the project with the portfolio user included
            var createdProject = await _context.Projects
                .Include(p => p.PortfolioUser)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, createdProject);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the project.", error = ex.Message });
        }
    }

    // PUT: api/Projects/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, Project project)
    {
        try
        {
            if (id != project.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(project.Title))
            {
                return BadRequest(new { message = "Title is required." });
            }

            if (string.IsNullOrWhiteSpace(project.Description))
            {
                return BadRequest(new { message = "Description is required." });
            }

            if (project.PortfolioUserId <= 0)
            {
                return BadRequest(new { message = "Valid PortfolioUserId is required." });
            }

            // Verify that the portfolio user exists
            var portfolioUser = await _context.PortfolioUsers.FindAsync(project.PortfolioUserId);
            if (portfolioUser == null)
            {
                return BadRequest(new { message = $"Portfolio user with ID {project.PortfolioUserId} not found." });
            }

            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
            {
                return NotFound(new { message = $"Project with ID {id} not found." });
            }

            // Store the original user ID for cache invalidation
            var originalUserId = existingProject.PortfolioUserId;

            // Update properties
            existingProject.Title = project.Title;
            existingProject.Description = project.Description;
            existingProject.ImageUrl = project.ImageUrl;
            existingProject.PortfolioUserId = project.PortfolioUserId;

            _context.Entry(existingProject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
                // Invalidate caches for both original and new user (in case user changed)
                InvalidateUserCaches(originalUserId);
                if (originalUserId != project.PortfolioUserId)
                {
                    InvalidateUserCaches(project.PortfolioUserId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound(new { message = $"Project with ID {id} not found." });
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
            return StatusCode(500, new { message = "An error occurred while updating the project.", error = ex.Message });
        }
    }

    // DELETE: api/Projects/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound(new { message = $"Project with ID {id} not found." });
            }

            var userId = project.PortfolioUserId;
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            // Invalidate caches for this user
            InvalidateUserCaches(userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while deleting the project.", error = ex.Message });
        }
    }

    // GET: api/Projects/statistics
    [HttpGet("statistics")]
    public async Task<ActionResult<object>> GetProjectStatistics()
    {
        try
        {
            const string cacheKey = "projects_statistics";
            
            if (_cache.TryGetValue(cacheKey, out object? cachedStats))
            {
                return Ok(cachedStats);
            }

            var totalProjects = await _context.Projects.CountAsync();
            var projectsByUser = await _context.Projects
                .GroupBy(p => p.PortfolioUserId)
                .Select(g => new { PortfolioUserId = g.Key, Count = g.Count() })
                .ToListAsync();

            var averageProjectsPerUser = projectsByUser.Any() ? projectsByUser.Average(x => x.Count) : 0;

            var statistics = new
            {
                TotalProjects = totalProjects,
                TotalUsers = projectsByUser.Count,
                AverageProjectsPerUser = Math.Round(averageProjectsPerUser, 2),
                ProjectsByUser = projectsByUser
            };

            // Cache statistics for 15 minutes (shorter than detailed data)
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(cacheKey, statistics, cacheOptions);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving project statistics.", error = ex.Message });
        }
    }

    private bool ProjectExists(int id)
    {
        return _context.Projects.Any(e => e.Id == id);
    }

    private void InvalidateUserCaches(int userId)
    {
        _cache.Remove($"projects_user_{userId}");
        _cache.Remove("all_projects");
        _cache.Remove("projects_statistics");
    }
}
