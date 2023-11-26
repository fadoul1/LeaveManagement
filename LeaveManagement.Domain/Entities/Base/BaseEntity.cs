namespace LeaveManagement.Domain.Entities.Base;

public class BaseEntity : IBaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}
