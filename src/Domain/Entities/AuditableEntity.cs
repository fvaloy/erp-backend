namespace Domain.Entities;

public class AuditableEntity : IEntity, IDeletable
{
    public string Id { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
    public int Version { get; set; } = 1;
}

public interface IEntity
{
    string Id { get; set; }
}

public interface IDeletable
{
    bool IsDeleted { get; set; }
}