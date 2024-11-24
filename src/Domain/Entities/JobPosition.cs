namespace Domain.Entities;

public sealed class JobPosition : AuditableEntity
{
    public string PositionBlueprintId { get; set; } = string.Empty;
    public PositionBlueprint PositionBlueprint { get; set; } = null!;
    public bool IsVancant { get; set; }
}