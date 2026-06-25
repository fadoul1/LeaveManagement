using LeaveManagement.Domain.Contracts.Services;

namespace LeaveManagement.Services.DateTimeServices;

public class SystemTimeProvider : ITimeProvider
{
    public DateTime Today => DateTime.Today;
    public DateTime UtcNow => DateTime.UtcNow;
}
