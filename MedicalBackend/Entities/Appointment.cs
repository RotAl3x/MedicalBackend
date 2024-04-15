using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalBackend.Entities;

public class Appointment:Entity
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    [ForeignKey(("RoomOrDevice"))]
    public Guid RoomOrDeviceId { get; set; }
    public RoomOrDevice RoomOrDevice { get; set; }
    [ForeignKey(("ApplicationUser"))]
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    [ForeignKey(("MedicalService"))]
    public Guid MedicalServiceId { get; set; }
    public MedicalService MedicalService { get; set; }
    public string Phone { get; set; }
    [ForeignKey(("Disease"))]
    public Guid DiseaseId { get; set; }
    public Disease Disease { get; set; }
}