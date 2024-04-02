using Microsoft.AspNetCore.Mvc;

namespace MedicalBackend.Controllers;


[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    [HttpGet("test")]
    public async Task<ActionResult> GetAllAssignedCourseUser()
    {
        return Ok();
    }
}