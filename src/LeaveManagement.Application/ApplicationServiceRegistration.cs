using FluentValidation;
using LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;
using LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;
using LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagement.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateEmployeeHandler).Assembly));
        services.AddScoped<IValidator<CreateEmployeeCommand>, CreateEmployeeValidator>();
        services.AddScoped<IValidator<UpdateEmployeeCommand>, UpdateEmployeeValidator>();
        services.AddScoped<IValidator<CreateLeaveCommand>, CreateLeaveValidator>();

        return services;
    }
}