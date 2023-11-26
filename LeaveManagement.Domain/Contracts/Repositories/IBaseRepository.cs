using LeaveManagement.Domain.Entities.Base;

namespace LeaveManagement.Domain.Contracts.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(long id);
    Task<List<T>> GetAllAsync();
    Task<T> CreateAsync(T obj);
    Task<T> UpdateAsync(T obj);
    Task<bool> DeleteAsync(long id);
}