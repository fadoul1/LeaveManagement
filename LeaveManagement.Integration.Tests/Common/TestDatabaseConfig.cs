using LeaveManagement.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Tests.IntegrationTests.Common;

internal static class TestDatabaseConfig
{
    public static DbContextOptions<ApplicationContext> GetInMemoryDatabaseOptions(string databaseName)
    {
        var builder = new DbContextOptionsBuilder<ApplicationContext>();
        builder.UseInMemoryDatabase(databaseName);
        return builder.Options;
    }

    public static void SetupInMemoryDatabase(ApplicationContext dbContext)
    {
        dbContext.Database.EnsureCreated();
    }
}
