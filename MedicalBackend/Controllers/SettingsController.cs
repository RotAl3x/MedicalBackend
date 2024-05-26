using MedicalBackend.Entities;
using MedicalBackend.Repositories.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBackend.Controllers;


[ApiController]
[Route("/api/settings")]

public class SettingsController: ControllerBase
{
    private readonly IBaseRepository<Settings> _baseRepository;
    private readonly IConfiguration _configuration;

    public SettingsController(IBaseRepository<Settings> baseRepository, IConfiguration configuration)
    {
        _baseRepository = baseRepository;
        _configuration = configuration;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var response = await _baseRepository.GetAll();
        if (!response.Any())
        {
            return Ok();
        }
        return Ok(response[0]);
    }
    
    [HttpPut]
    [Authorize(Roles = "Admin",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> EditSettings([FromBody] Settings settings)
    {
        var response = await _baseRepository.Edit(settings);
        return Ok(response);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateSettings([FromBody] Settings settings)
    {
        var response = await _baseRepository.Create(settings);
        return Ok(response);
    }

    [HttpGet("doctor-initial-password")]
    [Authorize(Roles = "Admin",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetDoctorInitialPassword()
    {
        var password = _configuration.GetSection("Doctor:Password").Value;
        return Ok(password ?? "");
    }
    
    [HttpGet("photo/{name}")]
    public IActionResult Get([FromRoute] string name)
    {
        var path = $"../Images/{name}"; 
        var image = System.IO.File.OpenRead(path);
        return File(image, "image/jpeg");
    }

    [HttpPost("photo/upload-photo")]
    public async Task<ActionResult> SaveFile(IFormFile file)
    {
        string filename = "";
        try
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            filename = DateTime.Now.Ticks.ToString() + extension;
            var folderImages = "../Images";
            if (!Directory.Exists(folderImages))
            {
                Directory.CreateDirectory(folderImages);
            }
            var exactpath = Path.Combine(folderImages, filename);


            using (var stream = new FileStream(exactpath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(filename);
        }
        catch (Exception ex)
        {
            return BadRequest(("Eroare"));
        }
    }
}