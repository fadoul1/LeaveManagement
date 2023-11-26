using LeaveManagement.Application.Responses;
using MediatR;

namespace LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommand : IRequest<EmployeeResponse>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}
