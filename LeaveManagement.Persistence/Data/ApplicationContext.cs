using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Persistence.Data;

public class ApplicationContext : DbContext
{

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Leave> Leaves { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
          : base(options)
    {

    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        foreach(var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ((IBaseEntity)entry.Entity).CreatedAt = DateTime.Now;
                    ((IBaseEntity)entry.Entity).UpdatedAt = DateTime.Now;
                    break;

                case EntityState.Modified:
                    ((IBaseEntity)entry.Entity).UpdatedAt = DateTime.Now;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.CurrentValues["DeletedAt"] = DateTime.Now;
                    break;
            }
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
