using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly string _className = nameof(DeleteEmployeeCommandHandler);
    private readonly IBaseRepository<Employee> _employeeRepository;
    private readonly ILogger<DeleteEmployeeCommandHandler> _logger;

    public DeleteEmployeeCommandHandler(
        IBaseRepository<Employee> employeeRepository,
        ILogger<DeleteEmployeeCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var methodeName = nameof(Handle);
        try
        {
            _logger.LogInformation($"[{_className}][{methodeName}] Deletion of the employee with id: {request.EmployeeId}");
            await _employeeRepository.DeleteAsync(request.EmployeeId);
            _logger.LogInformation($"[{_className}][{methodeName}] Successfully deleted the employee with id: {request.EmployeeId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{_className}][{methodeName}] Exception: {ex}, Message: {ex.Message}");
        }
    }
}