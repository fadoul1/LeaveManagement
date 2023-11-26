using LeaveManagement.Domain.Entities.Base;

namespace LeaveManagement.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public virtual ICollection<Leave> Leaves { get; set; } = [];
}
