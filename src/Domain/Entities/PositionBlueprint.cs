namespace Domain.Entities;

public sealed class PositionBlueprint : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal MaxSalary { get; set; }
    public decimal MinSalary { get; set; }
}