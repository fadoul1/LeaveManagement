using LeaveManagement.Application.Responses;
using MediatR;

namespace LeaveManagement.Application.Features.Leaves.Queries.GetLeavesByEmployeeId;

public class GetLeavesByEmployeeIdQuery : IRequest<List<LeaveResponse>>
{
    public long EmployeeId { get; set; }
}
