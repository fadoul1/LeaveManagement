using LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;
using LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Entities;

namespace LeaveManagement.Application.Mappers;

public static class EmployeeMapper
{
    public static Employee ToEmployee(this CreateEmployeeCommand command)
    {
        return new Employee
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
        };
    }

    public static Employee ToEmployee(this UpdateEmployeeCommand command)
    {
        return new Employee
        {
            Id = command.EmployeeId,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
        };
    }

    public static EmployeeResponse ToEmployeeResponse(this Employee employee)
    {
        return new EmployeeResponse
        {
            EmployeeId = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Success = true,
        };
    }
}
