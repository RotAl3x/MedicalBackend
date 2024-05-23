using MedicalBackend.Entities;
using MedicalBackend.Utils;

namespace MedicalBackend.Repositories.Abstractions;

public interface IIdentityRepository
{
    Task<Session> Login(LoginRequest request);
    
    Task<string> Register(RegisterRequest request, string role);

    Task<string?> ChangePassword(ChangePasswordRequest request, ApplicationUser user);
    
    Task<string?> Delete(ApplicationUser user);

}