using System.ComponentModel.DataAnnotations;

namespace MedicalBackend.Utils;

public class RegisterRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    public string FirstName { get; set; }
        
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public string PhoneNumber { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; }
}