using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Persistence.Data;
using LeaveManagement.Persistence.Repositories;
using LeaveManagement.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagement.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("TicketManagementConnectionString")));

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ILeaveRepository, LeaveRepository>();

        return services;
    }
}