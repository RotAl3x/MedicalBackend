namespace MedicalBackend.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime Start { get; set; }
    public DateTime? StartDateForMessage { get; set; }
    public DateTime End { get; set; }
    public Guid? RoomOrDeviceId { get; set; }
    public string? ApplicationUserId { get; set; }
    public Guid? MedicalServiceId { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public Guid? DiseaseId { get; set; }
    public bool IsFreeDay { get; set; }
    public bool IsDoctorFreeDay { get; set; }
}