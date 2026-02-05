using FluentValidation;
using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeResponse?>
{
    private readonly string _className = nameof(UpdateEmployeeHandler);
    private readonly IBaseRepository<Employee> _employeeRepository;
    private readonly IValidator<UpdateEmployeeCommand> _updateEmployeeCommandValidator;
    private readonly ILogger<UpdateEmployeeHandler> _logger;


    public UpdateEmployeeHandler(
        IBaseRepository<Employee> employeeRepository,
        IValidator<UpdateEmployeeCommand> updateEmployeeCommandValidator,
        ILogger<UpdateEmployeeHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _updateEmployeeCommandValidator = updateEmployeeCommandValidator;
        _logger = logger;
    }

    public async Task<EmployeeResponse?> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var methodeName = nameof(Handle);
        try
        {
            _logger.LogInformation($"[{_className}][{methodeName}] Validation of the entries");

            var result = await _updateEmployeeCommandValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                var resultErrors = result.Errors;
                string errors = string.Empty;

                foreach (var error in resultErrors)
                {
                    errors += $"Property {error.PropertyName} failed Validation. Error was: {error.ErrorMessage} \n";
                }
                _logger.LogInformation($"[{_className}][{methodeName}] Entries not valid, with errors: {errors}");

                return new EmployeeResponse
                {
                    Success = false,
                    ValidationErrors = errors
                };
            }

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully validated entries");

            _logger.LogInformation($"[{_className}][{methodeName}] Updating of the employee with id: {request.EmployeeId}");

            var employee = request.ToEmployee();
            var employeeCreated = await _employeeRepository.UpdateAsync(employee);

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully updated the user with id: {request.EmployeeId}");
            var employeeResponse = employeeCreated.ToEmployeeResponse();
            return employeeCreated.ToEmployeeResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{_className}][{methodeName}] Exception: {ex}, Message: {ex.Message}");
            return new EmployeeResponse
            {
                Success = false
            };
        }
    }
}