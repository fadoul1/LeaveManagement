using LeaveManagement.Domain.Entities.Base;
using LeaveManagement.Domain.Enumerations;

namespace LeaveManagement.Domain.Entities;

public class Leave : BaseEntity
{
    public LeaveTypeEnum Type { get; set; }
    public LeaveStatusEnum Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }
}