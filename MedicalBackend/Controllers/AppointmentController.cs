using MedicalBackend.DTOs;
using MedicalBackend.Entities;
using MedicalBackend.Hub;
using MedicalBackend.Hub.Abstractions;
using MedicalBackend.Repositories.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MedicalBackend.Controllers;

[ApiController]
[Route("/api/appointment")]
public class AppointmentController : ControllerBase
{
    private readonly IBaseRepository<Appointment> _baseRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private IHubContext<MessageHub, IMessageHubClient> messageHub;

    public AppointmentController(IBaseRepository<Appointment> baseRepository,
        IAppointmentRepository appointmentRepository, IHubContext<MessageHub, IMessageHubClient> _messageHub)
    {
        _baseRepository = baseRepository;
        _appointmentRepository = appointmentRepository;
        messageHub = _messageHub;
    }

    [HttpGet("getAll")]
    public async Task<ActionResult> GetAll()
    {
        var response = await _baseRepository.GetAll();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var response = await _baseRepository.GetById(id);
        if (response.Id == null)
        {
            return BadRequest();
        }

        return Ok(response);
    }
    
    [HttpGet("{roomId}/{doctorId}")]
    [HttpGet("roomId/{roomId}")]
    [HttpGet("doctorId/{doctorId}")]
    public async Task<ActionResult> GetByRoomIdOrDoctorId(Guid? roomId, string? doctorId)
    {
        var response = await _appointmentRepository.GetByRoomIdOrDoctorId(roomId, doctorId);
        if (!response.Any())
        {
            return BadRequest();
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Create(AppointmentDto obj)
    {
        var response = await _appointmentRepository.Create(obj);
        if (response.Id == null)
        {
            return BadRequest();
        }
        await messageHub.Clients.All.SendAppointmentToUser(response);
        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var response = await _appointmentRepository.Delete(id);
        if (response.Id == null)
        {
            return BadRequest();
        }
        await messageHub.Clients.All.SendAppointmentToUser(response);

        return Ok();
    }
}