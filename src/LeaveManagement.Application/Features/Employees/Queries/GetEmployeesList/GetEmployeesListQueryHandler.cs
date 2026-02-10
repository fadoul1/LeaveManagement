using LeaveManagement.Application.Mappers;
using LeaveManagement.Application.Responses;
using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Application.Features.Employees.Queries.GetEmployeesList;

public class GetEmployeesListQueryHandler : IRequestHandler<GetEmployeesListQuery, List<EmployeeResponse>>
{
    private readonly string _className = nameof(GetEmployeesListQueryHandler);
    private readonly IBaseRepository<Employee> _employeeRepository;
    private readonly ILogger<GetEmployeesListQueryHandler> _logger;

    public GetEmployeesListQueryHandler(
        IBaseRepository<Employee> employeeRepository,
        ILogger<GetEmployeesListQueryHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<List<EmployeeResponse>> Handle(GetEmployeesListQuery request, CancellationToken cancellationToken)
    {
        var methodeName = nameof(Handle);
        try
        {
            _logger.LogInformation($"[{_className}][{methodeName}] Retriving employees from the database");

            var employees = await _employeeRepository.GetAllAsync();
            var employeesResponseList = employees.
                Select(e => e.ToEmployeeResponse())
                .ToList();

            _logger.LogInformation($"[{_className}][{methodeName}] Successfully retrieved employees from the database");

            return employeesResponseList;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{_className}][{methodeName}] Exception: {ex}, Message: {ex.Message}");
            return [];
        }

    }
}