using LeaveManagement.Domain.Contracts.Services;
using LeaveManagement.Services.DateTimeServices;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagement.ExternalServices;

public static class ExternalServicesRegistrations
{
    public static void AddExternalServices(this IServiceCollection services)
    {
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
    }
}
