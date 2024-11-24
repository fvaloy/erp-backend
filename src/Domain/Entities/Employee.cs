namespace Domain.Entities;

public sealed class Employee : AuditableEntity
{
    public string PersonId { get; set; } = string.Empty;
    public Person Person { get; set; } = null!;
    public string JobPositionId { get; set; } = string.Empty;
    public JobPosition JobPosition { get; set; } = null!;
    public decimal Salary { get; set; }
}