using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mira.Database.Entities;

public class StardewCharacter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Villager { get; set; } = ""; 
    public string Birthday { get; set; }  = ""; 
    public string Loves { get; set; }  = ""; 
    public string Likes { get; set; }  = ""; 
    public string Neutral { get; set; }  = ""; 
    public string Dislikes { get; set; }  = ""; 
    public string Hates { get; set; }  = ""; 
}
