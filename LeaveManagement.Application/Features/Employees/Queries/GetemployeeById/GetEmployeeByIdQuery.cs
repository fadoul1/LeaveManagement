using LeaveManagement.Application.Responses;
using MediatR;

namespace LeaveManagement.Application.Features.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQuery : IRequest<EmployeeResponse>
{
    public long EmployeeId { get; set; }
}
