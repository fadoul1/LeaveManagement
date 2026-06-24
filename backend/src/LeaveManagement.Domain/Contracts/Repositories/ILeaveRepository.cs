using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enumerations;

namespace LeaveManagement.Domain.Contracts.Repositories;

public interface ILeaveRepository : IBaseRepository<Leave>
{
    Task<List<Leave>> GetEmployeeLeaveAsync(long employeeId);
    Task<List<Leave>> GetLeaveByStatusAsync(LeaveStatusEnum status);
    Task<List<Leave>> GetLeaveByTypeAsync(LeaveTypeEnum type);
}
