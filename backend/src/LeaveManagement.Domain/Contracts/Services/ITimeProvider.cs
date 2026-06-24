namespace LeaveManagement.Domain.Contracts.Services;

public interface ITimeProvider
{
    DateTime Today { get; }
    DateTime UtcNow { get; }
}
