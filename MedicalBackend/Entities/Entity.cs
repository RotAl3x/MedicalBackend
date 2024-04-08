namespace MedicalBackend.Entities;

public class Entity:IEntity
{
    public Guid Id { get; set; }
    public int Order { get; set; }
}