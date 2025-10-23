using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shared.Models;
using static System.Net.WebRequestMethods;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly SkillSnapContext _context;
        public SeedController(SkillSnapContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Seed()
        {
            // Ensure the DbSet name matches the context property
            if (_context.PortfolioUsers.Any())
            {
                return BadRequest("Database has already been seeded.");
            }
            var user = new PortfolioUser
            {
                Name = "Jordan Developer",
                Bio = "Full-stack developer passionate about learning new tech.",
                ProfileImageUrl = "https://example.com/images/jordan.png",
                Projects = new List<Project>
                {
                    new Project { Title = "Task Tracker", Description = "Manage tasks effectively.", ImageUrl = "https://example.com/images/task.png" },
                    new Project { Title = "Weather App", Description = "Forecast weather using APIs.", ImageUrl = "https://example.com/images/weather.png" }
                },
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Level = "Advanced" },
                    new Skill { Name = "Blazor", Level = "Intermediate" }
                }
            };
            _context.PortfolioUsers.Add(user);
            _context.SaveChanges();
            return Ok("Sample data inserted.");

        }
    }
}
