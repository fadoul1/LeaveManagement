using MediatR;

namespace LeaveManagement.Application.Features.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommand : IRequest
{
    public long EmployeeId { get; set; }
}
