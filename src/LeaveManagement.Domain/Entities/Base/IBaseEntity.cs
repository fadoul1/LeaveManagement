namespace LeaveManagement.Domain.Entities.Base;

public interface IBaseEntity
{
    long Id { get; set; }
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
    DateTimeOffset DeletedAt { get; set; }
}
