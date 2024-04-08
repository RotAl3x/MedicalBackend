namespace MedicalBackend.Entities;

public interface IEntity
{
    public Guid Id { get; set; }
    public int Order { get; set; }
}