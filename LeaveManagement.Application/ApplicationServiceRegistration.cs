using Microsoft.Extensions.DependencyInjection;
using LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;
using FluentValidation;
using LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;
using LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;

namespace LeaveManagement.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateEmployeeHandler).Assembly));

        services.AddScoped<IValidator<CreateEmployeeCommand>, CreateEmployeeValidator>();
        services.AddScoped<IValidator<UpdateEmployeeCommand>, UpdateEmployeeValidator>();

        services.AddScoped<IValidator<CreateLeaveCommand>, CreateLeaveValidator>();

        return services;
    }
}