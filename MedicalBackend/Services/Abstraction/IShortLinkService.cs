namespace MedicalBackend.Services.Abstraction;

public interface IShortLinkService
{
    Task<string?> ShortLink(string link);
}