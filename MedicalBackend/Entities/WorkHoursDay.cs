namespace MedicalBackend.Entities;

public class WorkHoursDay : Entity
{
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime StartHour { get; set; }
    public DateTime EndHour { get; set; }
}