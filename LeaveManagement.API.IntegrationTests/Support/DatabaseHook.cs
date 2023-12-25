using LeaveManagement.Persistence.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagement.API.IntegrationTests.Support;

[Binding]
internal class DatabaseHook
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public DatabaseHook(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [AfterScenario]
    public void AfterScenario()
    {
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationContext>();

        dbContext.Database.EnsureDeleted();
    }
}
