using Microsoft.AspNetCore.Identity;

namespace MedicalBackend.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public override string ToString()
    {
        return UserName;
    }
}