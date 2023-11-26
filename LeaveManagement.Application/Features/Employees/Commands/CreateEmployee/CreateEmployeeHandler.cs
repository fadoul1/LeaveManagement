using FluentValidation;
using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, EmployeeResponse?>
{
    private readonly string _className = nameof(CreateEmployeeHandler);
    private readonly IBaseRepository<Employee> _employeeRepository;
    private readonly IValidator<CreateEmployeeCommand> _createEmployeeCommandValidator;
    private readonly ILogger<CreateEmployeeHandler> _logger;


    public CreateEmployeeHandler(
        IBaseRepository<Employee> employeeRepository,
        IValidator<CreateEmployeeCommand> createEmployeeCommandValidator,
        ILogger<CreateEmployeeHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _createEmployeeCommandValidator = createEmployeeCommandValidator;
        _logger = logger;
    }

    public async Task<EmployeeResponse?> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var methodeName = nameof(Handle);
        try
        {
            _logger.LogInformation($"[{_className}][{methodeName}] Validation of the entries");

            var result = await _createEmployeeCommandValidator.ValidateAsync(request);
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

            _logger.LogInformation($"[{_className}][{methodeName}] Creation of the employee");

            var employee = request.ToEmployee();
            var employeeCreated = await _employeeRepository.CreateAsync(employee);

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully created the user");
   
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
