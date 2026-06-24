using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enumerations;

namespace LeaveManagement.Application.Responses;

public class EmployeeResponse : BaseResponse
{
    public long EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
