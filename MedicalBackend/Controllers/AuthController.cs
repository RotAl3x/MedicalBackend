using AutoMapper;
using MedicalBackend.Entities;
using MedicalBackend.Repositories.Abstractions;
using MedicalBackend.Utils;
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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _identityRepository.Register(request,"User");
        if (response == null)
        {
            return BadRequest("Verifică formularul");
        }

        return Ok(response);
    }
}