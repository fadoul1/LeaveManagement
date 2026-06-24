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
        await RestartIdentitySequences(conn);
    }

    /// <summary>
    /// Respawner's WithReseed may silently fail to reset PostgreSQL IDENTITY sequences
    /// due to table name string-matching issues in its reseed CTE.
    /// This method forces a RESTART IDENTITY on all identity columns to ensure consistent IDs across scenarios.
    /// See: https://github.com/jbogard/Respawn/issues/141
    /// </summary>
    private static async Task RestartIdentitySequences(NpgsqlConnection conn)
    {
        const string findIdentitySequencesSql =
            """
            SELECT pg_get_serial_sequence(table_schema || '."' || table_name || '"', column_name)
            FROM information_schema.columns
            WHERE table_schema = 'public' AND is_identity = 'YES'
            """;

        var sequenceNames = new List<string>();

        // Retrieve the names of all sequences backing IDENTITY columns
        await using (var cmd = new NpgsqlCommand(findIdentitySequencesSql, conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(0))
                    sequenceNames.Add(reader.GetString(0));
            }
        }

        // Reset each sequence so the next inserted row starts at ID 1
        foreach (var sequence in sequenceNames)
        {
            await using var cmd = new NpgsqlCommand($"ALTER SEQUENCE {sequence} RESTART WITH 1", conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        await Task.CompletedTask;
    }
}
