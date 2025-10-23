using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Shared.Models;

public class PortfolioUser
{
    [Key]
    public int Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Bio {get;set;} = string.Empty;
    public string? ProfileImageUrl {get;set;}
    
    // Foreign key to ApplicationUser
    public string? UserId {get;set;}
    public ApplicationUser? User {get;set;}

    [JsonIgnore]
    public List<Project> Projects {get;set;} = new();
    [JsonIgnore]
    public List<Skill> Skills {get;set;} = new();
}