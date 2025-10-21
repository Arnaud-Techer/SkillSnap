using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Shared.Models;

public class PortfolioUser
{
    [Key]
    public int Id {get;set;}
    public string Name {get;set;}
    public string Bio {get;set;}
    public string ProfileImageUrl {get;set;}

    public List<Project> Projects {get;set;}
    public List<Skill> Skills {get;set;}
}