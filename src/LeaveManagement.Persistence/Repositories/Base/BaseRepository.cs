using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities.Base;
using LeaveManagement.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Persistence.Repositories.Base;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly ApplicationContext DbContext;
    protected readonly DbSet<T> Entities;

    public BaseRepository(ApplicationContext context)
    {
        DbContext = context;
        Entities = DbContext.Set<T>();
    }


    public async Task<T> GetByIdAsync(long id)
    {
        var entity = await Entities.SingleOrDefaultAsync(o => o.Id == id);
        return entity ?? throw new Exception("The " + typeof(T).Name + " with Id:" + id + " Not Found");
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await Entities.Where(o => o.DeletedAt == default).ToListAsync();
    }

    public async Task<T> CreateAsync(T obj)
    {
        obj.CreatedAt = DateTimeOffset.UtcNow;
        Entities.Add(obj);

        await DbContext.SaveChangesAsync();
        return obj;
    }

    public async Task<T> UpdateAsync(T obj)
    {
        obj.UpdatedAt = DateTimeOffset.UtcNow;
        Entities.Update(obj);

        await DbContext.SaveChangesAsync();
        return obj;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var obj = await GetByIdAsync(id);
        obj.DeletedAt = DateTimeOffset.UtcNow;
        Entities.Update(obj);

        return await DbContext.SaveChangesAsync() > 0;
    }
}
