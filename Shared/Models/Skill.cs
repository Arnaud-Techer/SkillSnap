using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Shared.Models;

public class Skill
{
    [Key]
    public int Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Level {get;set;} = string.Empty;

    // foreign key
    [ForeignKey("PortfolioUser")]
    public int PortfolioUserId {get;set;}
    public PortfolioUser? PortfolioUser {get;set;}
}