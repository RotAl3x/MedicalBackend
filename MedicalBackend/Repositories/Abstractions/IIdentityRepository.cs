using MedicalBackend.Entities;
using MedicalBackend.Utils;

namespace MedicalBackend.Repositories.Abstractions;

public interface IIdentityRepository
{
    Task<Session> Login(LoginModel model);

    Task<string?> Register(RegisterModel model, string role);

    Task<string?> ChangePassword(ChangePasswordModel model, ApplicationUser user);

    Task<string?> Delete(ApplicationUser user);
}