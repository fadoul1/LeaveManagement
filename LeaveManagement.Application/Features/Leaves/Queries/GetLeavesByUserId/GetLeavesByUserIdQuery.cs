using LeaveManagement.Application.Responses;
using MediatR;

namespace LeaveManagement.Application.Features.Leaves.Queries.GetLeavesByUserId;

public class GetLeavesByUserIdQuery : IRequest<List<LeaveResponse>>
{
    public long EmployeeId { get; set; }
}
