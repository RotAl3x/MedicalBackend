namespace MedicalBackend.Utils;

public class ChangePasswordModel
{
    public string currentPassword { get; set; }
    public string newPassword { get; set; }
    public string repeatPassword { get; set; }
}