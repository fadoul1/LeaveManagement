using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enumerations;
using LeaveManagement.Persistence.Data;
using LeaveManagement.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Persistence.Repositories;

public class LeaveRepository : BaseRepository<Leave>, ILeaveRepository
{
    private readonly ApplicationContext _context;

    public LeaveRepository(ApplicationContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Leave>> GetEmployeeLeaveAsync(long employeeId)
    {
        return await _context.Leaves
            .Where(l => l.EmployeeId == employeeId)
            .Include(l => l.Employee)
            .ToListAsync();
    }

    public async Task<List<Leave>> GetLeaveByStatusAsync(LeaveStatusEnum status)
    {
        return await _context.Leaves
            .Where(l => l.Status == status)
            .Include(l => l.Employee)
            .ToListAsync();
    }

    public async Task<List<Leave>> GetLeaveByTypeAsync(LeaveTypeEnum type)
    {
        return await _context.Leaves
            .Where(l => l.Type == type)
            .Include(l => l.Employee)
            .ToListAsync();
    }
}
