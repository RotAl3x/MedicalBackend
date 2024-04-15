namespace MedicalBackend.Entities;

public class Entity:IEntity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; } = false;
}