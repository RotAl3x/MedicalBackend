namespace MedicalBackend.Entities;

public class Settings : Entity
{
    public decimal Lat { get; set; }
    public decimal Lng { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string LinkFacebook { get; set; }
    public string LinkInstagram { get; set; }
    public string WorkingHours { get; set; }
}