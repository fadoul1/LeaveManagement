using LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Helpers;

namespace LeaveManagement.Application.Mappers;

public static class LeaveMapper
{
    public static Leave ToLeave(this CreateLeaveCommand command)
    {
        return new Leave
        {
            Type = command.Type,
            Status = command.Status,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Reason = command.Reason,
            EmployeeId = command.EmployeeId,
        };
    }

    public static LeaveResponse ToLeaveResponse(this Leave leave)
    {
        return new LeaveResponse
        {
            LeaveId = leave.Id,
            Type = leave.Type.GetDescription(),
            Status = leave.Status.GetDescription(),
            StartDate = leave.StartDate.ToString("dd/MM/yyyy"),
            EndDate = leave.EndDate.ToString("dd/MM/yyyy"),
            Reason = leave.Reason,
            EmployeeId = leave.EmployeeId,
            EmployeeName =
                leave.Employee != null
                    ? $"{leave.Employee.FirstName} {leave.Employee.LastName}"
                    : string.Empty,
            Success = true,
        };
    }
}
