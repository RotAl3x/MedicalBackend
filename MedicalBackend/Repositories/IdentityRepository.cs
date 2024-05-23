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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public IdentityRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Session> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return new Session();
        }

        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!validPassword)
        {
            return new Session();
        }

        var roles = await _userManager.GetRolesAsync(user);

        var session = new Session
        {
            UserId = user.Id,
            TokenType = "Bearer",
            Token = GenerateToken(user, roles.FirstOrDefault()),
            UserName = user.UserName,
            Role = roles.FirstOrDefault(),
        };

        return session;
    }

    public async Task<string?> Register(RegisterRequest request, string role)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true,
            Created = DateTime.Now.ToUniversalTime(),
            Updated = DateTime.Now.ToUniversalTime(),
        };

        var createResult = await _userManager.CreateAsync(user, _configuration.GetSection("Doctor:Password").Value);

        if (!createResult.Succeeded)
        {
            return null;
        }

        var addToRole = await _userManager.AddToRoleAsync(user, role);

        if (!addToRole.Succeeded)
        {
            return null;
        }

        return "User created successfully";
    }

    public async Task<string?> ChangePassword(ChangePasswordRequest request, ApplicationUser user)
    {
        if (!request.newPassword.Equals(request.repeatPassword))
        {
            return null;
        }

        var changePassword =
            await _userManager.ChangePasswordAsync(user, request.currentPassword, request.newPassword);

        if (!changePassword.Succeeded)
        {
            return null;
        }

        return "Password has been changed";
    }

    private string GenerateToken(ApplicationUser newUser, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JWT:Secret").Value);
        Console.WriteLine(_configuration.GetSection("JWT:Issuer").Value);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, newUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.GetSection("JWT:Issuer").Value),
                new Claim("id", newUser.Id),
                new Claim(ClaimTypes.Role, role)
            }),
            Issuer = _configuration.GetSection("JWT:Issuer").Value,
            Expires = DateTime.UtcNow.AddDays(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}