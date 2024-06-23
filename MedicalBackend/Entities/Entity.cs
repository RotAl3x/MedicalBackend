namespace MedicalBackend.Entities;

public class Entity : IEntity
{
    public bool IsDeleted { get; set; } = false;
    public Guid Id { get; set; }
}