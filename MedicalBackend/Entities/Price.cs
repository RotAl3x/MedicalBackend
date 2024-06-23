namespace MedicalBackend.Entities;

public class Price : Entity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal PriceForOne { get; set; }

    public int NumberOfMeets { get; set; }

    public decimal PriceForAllMeets { get; set; }
}