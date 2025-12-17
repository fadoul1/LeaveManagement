using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Persistence.Data;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Leave> Leaves { get; set; }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        foreach(var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ((IBaseEntity)entry.Entity).CreatedAt = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:
                    ((IBaseEntity)entry.Entity).UpdatedAt = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.CurrentValues["DeletedAt"] = DateTimeOffset.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
