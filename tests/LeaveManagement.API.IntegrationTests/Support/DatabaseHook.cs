using LeaveManagement.Persistence.Data;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;

namespace LeaveManagement.API.IntegrationTests.Support;

[Binding]
internal class DatabaseHook(CustomWebApplicationFactory<Program> factory)
{
    private static Respawner? s_respawner;
    private static bool s_databaseInitialized;
    private static readonly Lock Lock = new();

    [BeforeScenario(Order = 1)]
    public async Task BeforeScenario()
    {
        lock (Lock)
        {
            if (!s_databaseInitialized)
            {
                using var scope = factory.Services.CreateScope();
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ApplicationContext>();
                
                dbContext.Database.EnsureCreated();
                s_databaseInitialized = true;
            }
        }

        if (s_respawner == null)
        {
            await using var connection = new NpgsqlConnection(factory.ConnectionString);
            await connection.OpenAsync();
            s_respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                TablesToIgnore = ["__EFMigrationsHistory"],
                DbAdapter = DbAdapter.Postgres,
                WithReseed = true
            });
        }

        // Reset database before each scenario
        await ResetDatabaseConnection();
        SeedDatabaseForTests();
    }

    private void SeedDatabaseForTests()
    {
        using var seedScope = factory.Services.CreateScope();
        var seedServices = seedScope.ServiceProvider;
        var seedDbContext = seedServices.GetRequiredService<ApplicationContext>();
        Utilities.InitializeLeavesForTests(seedDbContext);
    }

    private async Task ResetDatabaseConnection()
    {
        await using var conn = new NpgsqlConnection(factory.ConnectionString);
        await conn.OpenAsync();
        await s_respawner!.ResetAsync(conn);
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        await Task.CompletedTask;
    }
}
