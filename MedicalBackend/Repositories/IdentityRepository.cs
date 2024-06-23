using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedicalBackend.Entities;
using MedicalBackend.Repositories.Abstractions;
using MedicalBackend.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MedicalBackend.Repositories;

public class IdentityRepository : IIdentityRepository
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Session> Login(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null) return new Session();

        var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!validPassword) return new Session();

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count > 1) roles = roles.Where(r => r != "User").ToList();

        var session = new Session
        {
            UserId = user.Id,
            UserName = user.UserName ?? "",
            TokenType = "Bearer",
            Token = GenerateToken(user, roles.FirstOrDefault() ?? "User"),
            Role = roles.FirstOrDefault() ?? "User"
        };

        return session;
    }

    public async Task<string?> Register(RegisterModel model, string role)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = true,
            Created = DateTime.Now.ToUniversalTime(),
            Updated = DateTime.Now.ToUniversalTime()
        };

        var doctorPassword = _configuration.GetSection("Doctor:Password").Value;

        if (doctorPassword is null) return null;

        var createResult = await _userManager.CreateAsync(user, doctorPassword);

        if (!createResult.Succeeded) return null;

        var addToRole = await _userManager.AddToRoleAsync(user, role);

        if (!addToRole.Succeeded) return null;

        return "User creat";
    }

    public async Task<string?> ChangePassword(ChangePasswordModel model, ApplicationUser user)
    {
        if (!model.newPassword.Equals(model.repeatPassword)) return null;

        var changePassword =
            await _userManager.ChangePasswordAsync(user, model.currentPassword, model.newPassword);

        if (!changePassword.Succeeded) return null;

        return "Parola a fost schimbată";
    }

    public async Task<string?> Delete(ApplicationUser user)
    {
        var deleteAccount = await _userManager.DeleteAsync(user);

        if (!deleteAccount.Succeeded) return null;

        return "User șters";
    }

    private string GenerateToken(ApplicationUser newUser, string role)
    {
        var jwtSecret = _configuration.GetSection("JWT:Secret").Value;
        var jwtIssuer = _configuration.GetSection("JWT:Issuer").Value;
        if (jwtSecret is null || jwtIssuer is null) return "";
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, newUser.UserName ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iss, jwtIssuer),
                new("id", newUser.Id),
                new(ClaimTypes.Role, role)
            }),
            Issuer = jwtIssuer,
            Expires = DateTime.UtcNow.AddDays(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}