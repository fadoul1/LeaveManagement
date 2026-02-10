using FluentValidation;
using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;

public partial class CreateEmployeeHandler(
    IBaseRepository<Employee> employeeRepository,
    IValidator<CreateEmployeeCommand> createEmployeeCommandValidator,
    ILogger<CreateEmployeeHandler> logger)
    : IRequestHandler<CreateEmployeeCommand, EmployeeResponse?>
{
    private const string ClassName = nameof(CreateEmployeeHandler);

    public async Task<EmployeeResponse?> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        const string methodeName = nameof(Handle);
        try
        {
            LogValidationOfTheEntries(logger, ClassName, methodeName);

            var result = await createEmployeeCommandValidator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
            {
                var resultErrors = result.Errors;
                var errors = string.Empty;

                foreach (var error in resultErrors)
                {
                    errors += $"Property {error.PropertyName} failed Validation. Error was: {error.ErrorMessage} \n";
                }
                LogEntriesNotValidWithErrorsErrors(logger, ClassName, methodeName, errors);

                return new EmployeeResponse
                {
                    Success = false,
                    ValidationErrors = errors
                };
            }

            LogSuccessfullyValidatedEntries(logger, ClassName, methodeName);

            LogCreationOfTheEmployee(logger, ClassName, methodeName);

            var employee = request.ToEmployee();
            var employeeCreated = await employeeRepository.CreateAsync(employee);

            LogSuccessfullyCreateEmployee(logger, ClassName, methodeName);
   
            return employeeCreated.ToEmployeeResponse();
        }
        catch (Exception ex)
        {
            LogExceptionMessage(logger, ClassName, methodeName, ex, ex.Message);
            return new EmployeeResponse
            {
                Success = false
            };
        }
    }

    [LoggerMessage(LogLevel.Information, "[{className}][{methodeName}] Validation of the entries")]
    static partial void LogValidationOfTheEntries(ILogger<CreateEmployeeHandler> logger, string className, string methodeName);

    [LoggerMessage(LogLevel.Information, "[{className}][{MethodeName}] Entries not valid, with errors: {Errors}")]
    static partial void LogEntriesNotValidWithErrorsErrors(ILogger<CreateEmployeeHandler> logger, string className, string MethodeName, string Errors);

    [LoggerMessage(LogLevel.Information, "[{className}][{methodeName}] Successfully validated entries")]
    static partial void LogSuccessfullyValidatedEntries(ILogger<CreateEmployeeHandler> logger, string className, string methodeName);

    [LoggerMessage(LogLevel.Information, "[{className}][{methodeName}] Creation of the employee")]
    static partial void LogCreationOfTheEmployee(ILogger<CreateEmployeeHandler> logger, string className, string methodeName);

    [LoggerMessage(LogLevel.Information, "[{className}][{methodeName}] Successfully created the user")]
    static partial void LogSuccessfullyCreateEmployee(ILogger<CreateEmployeeHandler> logger, string className, string methodeName);

    [LoggerMessage(LogLevel.Error, "[{className}][{methodeName}] Exception: {exception}, Message: {exMessage}")]
    static partial void LogExceptionMessage(ILogger<CreateEmployeeHandler> logger, string className, string methodeName, Exception exception, string exMessage);
}
