using System.ComponentModel.DataAnnotations.Schema;
using MedicalBackend.Enums;

namespace MedicalBackend.Entities;

public class SendSmsQueue : Entity
{
    [ForeignKey("Appointment")] public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public DateTime SendAfterDate { get; set; }
    public string Message { get; set; }
    public int RetryCount { get; set; }
    public string? Sid { get; set; }
    public TwilioStatusEnum? Status { get; set; }
}