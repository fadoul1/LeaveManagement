using LeaveManagement.Application.Responses;
using MediatR;

namespace LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommand : IRequest<EmployeeResponse>
{
    public long EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
