using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Enumerations;
using MediatR;

namespace LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;

public class CreateLeaveCommand: IRequest<LeaveResponse>
{
    public LeaveTypeEnum Type { get; set; }
    public LeaveStatusEnum Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long EmployeeId { get; set; }
}
