using LeaveManagement.Application.Features.Employees.Queries.GetEmployeeById;
using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Employees.Queries.GetEmployeesList;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponse>
{
    private readonly string _className = nameof(GetEmployeeByIdQueryHandler);
    private readonly IBaseRepository<Employee> _employeeRepository;
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;

    public GetEmployeeByIdQueryHandler(
        IBaseRepository<Employee> employeeRepository,
        ILogger<GetEmployeeByIdQueryHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<EmployeeResponse> Handle(GetEmployeeByIdQuery query, CancellationToken cancellationToken)
    {
        var methodeName = nameof(Handle);
        try
        {
            _logger.LogInformation($"[{_className}][{methodeName}] Retriving employee with id {query.EmployeeId} from the database");

            var employee = await _employeeRepository.GetByIdAsync(query.EmployeeId);
            var employeeResponse = employee.ToEmployeeResponse();

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully retrieved employee with id {query.EmployeeId} from the database");

            return employeeResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{_className}][{methodeName}] Exception: {ex}, Message: {ex.Message}");
            return new EmployeeResponse();
        }
    }
}