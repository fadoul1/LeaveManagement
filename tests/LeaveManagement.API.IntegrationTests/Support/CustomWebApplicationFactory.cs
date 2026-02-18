using LeaveManagement.Domain.Contracts.Services;
using LeaveManagement.Persistence.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace LeaveManagement.API.IntegrationTests.Support;

internal class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("LeaveManagementDb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString => _dbContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(services =>
        {
            // Remove all existing DbContext registrations
            services.RemoveAll<ApplicationContext>();
            services.RemoveAll<DbContextOptions<ApplicationContext>>();
            services.RemoveAll<DbContextOptions>();

            // Add a PostgresSQL database for testing
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            // Use a fake TimeProvider so test dates are always in the future
            services.RemoveAll<ITimeProvider>();
            services.AddSingleton<ITimeProvider>(new FakeTimeProvider(new DateTime(2025, 12, 1)));
        });
    }

    public HttpClient GetAnonymousClient()
    {
        return CreateClient();
    }
}
