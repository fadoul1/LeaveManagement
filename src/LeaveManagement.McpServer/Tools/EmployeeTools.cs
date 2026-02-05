using System.ComponentModel;
using System.Text.Json;
using LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;
using LeaveManagement.Application.Features.Employees.Commands.DeleteEmployee;
using LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;
using LeaveManagement.Application.Features.Employees.Queries.GetEmployeeById;
using LeaveManagement.Application.Features.Employees.Queries.GetEmployeesList;
using MediatR;
using ModelContextProtocol.Server;

namespace LeaveManagement.McpServer.Tools;

[McpServerToolType]
public class EmployeeTools
{
    private readonly IMediator _mediator;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public EmployeeTools(IMediator mediator)
    {
        _mediator = mediator;
    }

    [McpServerTool, Description("Retrieves all active employees from the system. Returns a list of employees with their details including ID, name, email, and phone number.")]
    public async Task<string> GetAllEmployees()
    {
        var query = new GetEmployeesListQuery();
        var result = await _mediator.Send(query);
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool, Description("Retrieves a specific employee by their ID. Returns employee details if found, or an error if the employee does not exist.")]
    public async Task<string> GetEmployeeById(
        [Description("The unique identifier of the employee to retrieve")] long employeeId)
    {
        var query = new GetEmployeeByIdQuery { EmployeeId = employeeId };
        var result = await _mediator.Send(query);

        if (result == null || result.EmployeeId == 0)
        {
            return JsonSerializer.Serialize(new { success = false, error = $"Employee with ID {employeeId} not found" }, JsonOptions);
        }

        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool, Description("Creates a new employee in the system. Validates that email is properly formatted, and that first name, last name are at least 2 characters, and phone number is at least 8 characters.")]
    public async Task<string> CreateEmployee(
        [Description("Employee's first name (minimum 2 characters)")] string firstName,
        [Description("Employee's last name (minimum 2 characters)")] string lastName,
        [Description("Employee's email address (must be valid email format)")] string email,
        [Description("Employee's phone number (minimum 8 characters)")] string phoneNumber)
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };

        var result = await _mediator.Send(command);
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool, Description("Updates an existing employee's information. All fields must be provided. Validates that email is properly formatted, and that first name, last name are at least 2 characters, and phone number is at least 8 characters.")]
    public async Task<string> UpdateEmployee(
        [Description("The unique identifier of the employee to update")] long employeeId,
        [Description("Employee's first name (minimum 2 characters)")] string firstName,
        [Description("Employee's last name (minimum 2 characters)")] string lastName,
        [Description("Employee's email address (must be valid email format)")] string email,
        [Description("Employee's phone number (minimum 8 characters)")] string phoneNumber)
    {
        var command = new UpdateEmployeeCommand
        {
            EmployeeId = employeeId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };

        var result = await _mediator.Send(command);
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool, Description("Soft-deletes an employee from the system. The employee record is marked as deleted but not physically removed from the database.")]
    public async Task<string> DeleteEmployee(
        [Description("The unique identifier of the employee to delete")] long employeeId)
    {
        var command = new DeleteEmployeeCommand { EmployeeId = employeeId };
        await _mediator.Send(command);
        return JsonSerializer.Serialize(new { success = true, message = $"Employee {employeeId} has been deleted" }, JsonOptions);
    }
}
