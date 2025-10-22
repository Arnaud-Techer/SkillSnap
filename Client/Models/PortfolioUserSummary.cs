namespace Client.Models;

public class PortfolioUserSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public int ProjectCount { get; set; }
    public int SkillCount { get; set; }
}
