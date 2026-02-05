using LeaveManagement.Application;
using LeaveManagement.Persistence;
using LeaveManagement.Persistence.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace LeaveManagement.McpServer;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder(args);

            // Configure logging to stderr (stdout is reserved for MCP protocol)
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options =>
            {
                options.LogToStandardErrorThreshold = LogLevel.Trace;
            });

            // Load configuration from environment variables first, then appsettings
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Register Application layer services (MediatR, validators, AutoMapper)
            builder.Services.AddApplicationServices();

            // Register Persistence layer services (DbContext, repositories)
            builder.Services.AddPersistenceServices(builder.Configuration);

            // Configure MCP Server with stdio transport
            builder.Services
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            var host = builder.Build();

            // Validate database connection on startup
            await ValidateDatabaseConnectionAsync(host.Services);

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("LeaveManagement MCP Server starting...");

            await host.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error during startup: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            return 1;
        }
    }

    private static async Task ValidateDatabaseConnectionAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger.LogWarning("Cannot connect to database. Attempting to create...");
                await context.Database.EnsureCreatedAsync();
            }
            logger.LogInformation("Database connection validated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to validate database connection");
            throw;
        }
    }
}
