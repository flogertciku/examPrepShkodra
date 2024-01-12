#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace examPrep.Models;
public class Login
{    
   

    [Required]
    public string LoginUsername { get; set; }    
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string LoginPassword { get; set; } 
}
