using LeaveManagement.Application.Responses;
using MediatR;

namespace LeaveManagement.Application.Features.Employees.Queries.GetEmployeesList;

public class GetEmployeesListQuery : IRequest<List<EmployeeResponse>>
{
}
