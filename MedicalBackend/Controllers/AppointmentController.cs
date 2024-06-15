using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text;
using MedicalBackend.DTOs;
using MedicalBackend.Entities;
using MedicalBackend.Hub;
using MedicalBackend.Hub.Abstractions;
using MedicalBackend.Repositories.Abstractions;
using MedicalBackend.Services.Abstraction;
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
    private IHubContext<MessageHub, IMessageHubClient> _messageHub;
    private readonly ISendSmsQueueRepository _sendSmsQueueRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AppointmentController> _logger;
    private readonly IShortLinkService _shortLinkService;

    public AppointmentController(IBaseRepository<Appointment> baseRepository, IConfiguration configuration, ILogger<AppointmentController> logger,
        IAppointmentRepository appointmentRepository, IHubContext<MessageHub, IMessageHubClient> messageHub, IShortLinkService shortLinkService,
        ISendSmsQueueRepository sendSmsQueueRepository)
    {
        _baseRepository = baseRepository;
        _appointmentRepository = appointmentRepository;
        _messageHub = messageHub;
        _sendSmsQueueRepository = sendSmsQueueRepository;
        _configuration = configuration;
        _logger = logger;
        _shortLinkService = shortLinkService;
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

        return Ok(response);
    }

    [HttpGet("cabinet-free-days")]
    [Authorize(Roles = "Admin,Doctor",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> GetCabinetFreeDays()
    {
        var response = await _appointmentRepository.GetCabinetFreeDays();

        return Ok(response);
    }

    [HttpGet("doctor-free-days/{id}")]
    [Authorize(Roles = "Admin,Doctor",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> GetDoctorFreeDays(string id)
    {
        var response = await _appointmentRepository.GetDoctorFreeDays(id);

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

        await _messageHub.Clients.All.SendAppointmentToUser(response);
        if (obj.IsFreeDay || obj.IsDoctorFreeDay) return Ok(response);
        var dateToLocalTime = obj.StartDateForMessage ?? new DateTime();
        var dateOfStartAppointment = dateToLocalTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        var timeOfStartAppointment = dateToLocalTime.ToString("H:mm", CultureInfo.InvariantCulture);
        var frontendLink = _configuration.GetSection("FrontendLink").Value ?? "";
        
        //short a link
        var linkDeleteShorten = await _shortLinkService.ShortLink($"{frontendLink}/appointment/delete/{response.Id}");
        //can be send
        linkDeleteShorten = linkDeleteShorten?.Replace('.', 'I');
        
        var cabinetName = _configuration.GetSection("CabinetName").Value ?? "";
        await _sendSmsQueueRepository.Create(response.Id,
            $"Te așteptăm la {cabinetName} în data {dateOfStartAppointment}-{timeOfStartAppointment}, " +
            $"Dacă vrei să anulezi: {linkDeleteShorten}",
            DateTime.UtcNow);
        if (DateTime.UtcNow.AddHours(48) <= response.Start)
        {
            await _sendSmsQueueRepository.Create(response.Id,
                $"24 de ore până la programarea la {cabinetName}." +
                $"Anulezi: {linkDeleteShorten}",
                response.Start.AddHours(-24));
        }

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

        await _messageHub.Clients.All.SendAppointmentToUser(response);
        await _sendSmsQueueRepository.DeleteByAppointmentId(response.Id);

        return Ok();
    }
}