using MedicalBackend.Entities;
using MedicalBackend.Repositories.Abstractions;
using MedicalBackend.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBackend.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly IIdentityRepository _identityRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(UserManager<ApplicationUser> userManager, IIdentityRepository identityRepository)
    {
        _userManager = userManager;
        _identityRepository = identityRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var response = await _identityRepository.Login(model);
        if (response.Token == null) return BadRequest("Verifică formularul");

        return Ok(response);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var response = await _identityRepository.Register(model, "Doctor");
        if (response == null) return BadRequest("Verifică formularul");

        return Ok(response);
    }

    [HttpDelete("deleteAccount/{id}")]
    [Authorize(Roles = "Admin",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound("Eroare");

        var response = await _identityRepository.Delete(user);
        if (response == null) return BadRequest("Eroare");

        return Ok(response);
    }

    [HttpPost("changePassword")]
    [Authorize(Roles = "Admin,Doctor",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return NotFound("Eroare");

        var response = await _identityRepository.ChangePassword(model, user);
        if (response is null) return BadRequest("Verifică formularul");

        return Ok(response);
    }

    [HttpGet("getAllUsersByRole/{role}")]
    public async Task<IActionResult> GetAllUsers([FromRoute] string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);

        return Ok(users);
    }
}