using LeaveManagement.Domain.Contracts.Services;

namespace LeaveManagement.API.IntegrationTests.Support;

internal class FakeTimeProvider : ITimeProvider
{
    private readonly DateTime _fixedDate;

    public FakeTimeProvider(DateTime fixedDate)
    {
        _fixedDate = fixedDate;
    }

    public DateTime Today => _fixedDate.Date;
    public DateTime UtcNow => _fixedDate;
}
