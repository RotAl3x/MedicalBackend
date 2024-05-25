using System.ComponentModel.DataAnnotations.Schema;
using MedicalBackend.Enums;
using Microsoft.EntityFrameworkCore;
using Twilio.Rest.Api.V2010.Account;

namespace MedicalBackend.Entities;

public class SendSmsQueue : Entity
{
    [ForeignKey(("Appointment"))] public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime SendAfterDate { get; set; }
    public string Message { get; set; }
    public int RetryCount { get; set; }
    public string? Sid { get; set; }
    public TwilioStatusEnum? Status { get; set; }
}