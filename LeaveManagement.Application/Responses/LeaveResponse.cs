namespace LeaveManagement.Application.Responses;

public class LeaveResponse : BaseResponse
{
    public long LeaveId { get; set; }
    public string Type { get; set; } = string.Empty;    
    public string Status { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
}
