using MedicalBackend.Entities;
using MedicalBackend.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBackend.Controllers;


[ApiController]
[Route("/api/doctor")]
public class DoctorController : ControllerBase
{
    private readonly IBaseRepository<Doctor> _baseRepository;

    public DoctorController(IBaseRepository<Doctor> baseRepository)
    {
        _baseRepository = baseRepository;
    }
    
    [HttpGet("getAll")]
    public async Task<ActionResult> GetAll()
    {
        var response = await _baseRepository.GetAll();
        return Ok(response);
    }
    
    [HttpGet("${id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var response = await _baseRepository.GetById(id);
        if (response.Name == null)
        {
            return BadRequest();
        }
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult> Create(Doctor obj)
    {
        var response = await _baseRepository.Create(obj);
        return Ok(response);
    }
    
    [HttpPut]
    public async Task<ActionResult> Edit(Doctor obj)
    {
        var response = await _baseRepository.Edit(obj);
        if (response.Name == null)
        {
            return BadRequest();
        }
        return Ok(response);
    }
    
    [HttpDelete]
    public async Task<ActionResult> Delete(Guid id)
    {
        var response = await _baseRepository.Delete(id);
        if (response == null)
        {
            return BadRequest();
        }
        return Ok();
    }
}