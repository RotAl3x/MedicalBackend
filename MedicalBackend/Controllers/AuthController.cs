using AutoMapper;
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
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public AuthController(IMapper mapper, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IIdentityRepository identityRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _identityRepository = identityRepository;
        _mapper = mapper;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _identityRepository.Login(request);
        if (response.Token == null)
        {
            return BadRequest("Verifică formularul");
        }

        return Ok(response);
    }
    
    [HttpPost("register")]
    [Authorize(Roles = "Admin",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _identityRepository.Register(request,"Doctor");
        if (response == null)
        {
            return BadRequest("Verifică formularul");
        }

        return Ok(response);
    }
    
    [HttpDelete("deleteAccount/{id}")]
    [Authorize(Roles = "Admin",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Eroare");
        }
        
        var response = await _identityRepository.Delete(user);
        if (response == null)
        {
            return BadRequest("Eroare");
        }

        return Ok(response);
    }

    [HttpPost("changePassword")]
    [Authorize(Roles = "Admin,Doctor",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound("Error");
        }

        var response = await _identityRepository.ChangePassword(request, user);
        if (response is null)
        {
            return BadRequest("Verifică formularul");
        }

        return Ok(response);
    }

    [HttpGet("getAllUsersByRole/{role}")]
    public async Task<IActionResult> GetAllUsers([FromRoute] string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);

        if (!users.Any())
        {
            return NotFound("Nu există");
        }

        return Ok(users);
    }
}