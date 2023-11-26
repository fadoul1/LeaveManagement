namespace LeaveManagement.Domain.Entities.Base;

public interface IBaseEntity
{
    long Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    DateTime DeletedAt { get; set; }
}
