using System.ComponentModel.DataAnnotations;

namespace MedicalBackend.Utils;

public class ChangePasswordRequest
{
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string currentPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string newPassword { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string repeatPassword { get; set; }
}