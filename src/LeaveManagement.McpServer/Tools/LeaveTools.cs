using System.ComponentModel;
using System.Text.Json;
using LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;
using LeaveManagement.Application.Features.Leaves.Queries.GetLeavesByEmployeeId;
using LeaveManagement.Application.Features.Leaves.Queries.GetLeavesList;
using LeaveManagement.Domain.Enumerations;
using MediatR;
using ModelContextProtocol.Server;

namespace LeaveManagement.McpServer.Tools;

[McpServerToolType]
public class LeaveTools
{
    private readonly IMediator _mediator;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LeaveTools(IMediator mediator)
    {
        _mediator = mediator;
    }

    [McpServerTool, Description("Retrieves all leave requests from the system. Returns a list of leaves with employee information, dates, type, and status.")]
    public async Task<string> GetAllLeaves()
    {
        var query = new GetLeavesListQuery();
        var result = await _mediator.Send(query);
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool, Description("Retrieves all leave requests for a specific employee. Returns the employee's leave history including dates, types, and statuses.")]
    public async Task<string> GetLeavesByEmployeeId(
        [Description("The unique identifier of the employee whose leaves to retrieve")] long employeeId)
    {
        var query = new GetLeavesByEmployeeIdQuery { EmployeeId = employeeId };
        var result = await _mediator.Send(query);
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool, Description("Creates a new leave request for an employee. Validates that dates are in the future and employee exists. Leave types: SickLeave, AnnualLeave, Other. Leave statuses: InProgress, Finish.")]
    public async Task<string> CreateLeave(
        [Description("The type of leave: 'SickLeave', 'AnnualLeave', or 'Other'")] string leaveType,
        [Description("The status of the leave: 'InProgress' or 'Finish'")] string leaveStatus,
        [Description("The start date of the leave (format: yyyy-MM-dd, must be in the future)")] string startDate,
        [Description("The end date of the leave (format: yyyy-MM-dd, must be in the future)")] string endDate,
        [Description("The ID of the employee requesting leave")] long employeeId,
        [Description("Optional reason for the leave request")] string? reason = null)
    {
        if (!Enum.TryParse<LeaveTypeEnum>(leaveType, ignoreCase: true, out var typeEnum))
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = $"Invalid leave type: {leaveType}. Valid values are: SickLeave, AnnualLeave, Other"
            }, JsonOptions);
        }

        if (!Enum.TryParse<LeaveStatusEnum>(leaveStatus, ignoreCase: true, out var statusEnum))
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = $"Invalid leave status: {leaveStatus}. Valid values are: InProgress, Finish"
            }, JsonOptions);
        }

        if (!DateTime.TryParse(startDate, out var parsedStartDate))
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = $"Invalid start date format: {startDate}. Expected format: yyyy-MM-dd"
            }, JsonOptions);
        }

        if (!DateTime.TryParse(endDate, out var parsedEndDate))
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = $"Invalid end date format: {endDate}. Expected format: yyyy-MM-dd"
            }, JsonOptions);
        }

        var command = new CreateLeaveCommand
        {
            Type = typeEnum,
            Status = statusEnum,
            StartDate = parsedStartDate,
            EndDate = parsedEndDate,
            EmployeeId = employeeId,
            Reason = reason ?? string.Empty
        };

        var result = await _mediator.Send(command);
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool, Description("Returns information about available leave types and statuses in the system.")]
    public string GetLeaveEnumValues()
    {
        var enumInfo = new
        {
            leaveTypes = new[]
            {
                new { value = "SickLeave", description = "Leave due to illness" },
                new { value = "AnnualLeave", description = "Planned annual vacation" },
                new { value = "Other", description = "Other types of leave" }
            },
            leaveStatuses = new[]
            {
                new { value = "InProgress", description = "Leave request is ongoing or pending" },
                new { value = "Finish", description = "Leave has been completed" }
            }
        };

        return JsonSerializer.Serialize(enumInfo, JsonOptions);
    }
}
