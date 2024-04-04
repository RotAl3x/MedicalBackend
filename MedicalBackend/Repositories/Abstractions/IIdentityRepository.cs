using MedicalBackend.Utils;

namespace MedicalBackend.Repositories.Abstractions;

public interface IIdentityRepository
{
    Task<Session> Login(LoginRequest request);
    
    Task<string> Register(RegisterRequest request, string role);
}