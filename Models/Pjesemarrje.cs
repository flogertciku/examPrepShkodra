#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using examPrep.Models;
namespace examPrep.Models;
public class Pjesemarrje
{
    [Key]
    public int PjesemarrjeId { get; set; }
    public int? UserId {get;set;}
    public int? AktivitetiId {get;set;}
    
    [MinLength(4,ErrorMessage = "pershkrimi me i gjate se 4 shkronja")]
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public User? Pjesemarres {get;set;}
    public Aktiviteti? Aktiviteti {get;set;}

}
                
