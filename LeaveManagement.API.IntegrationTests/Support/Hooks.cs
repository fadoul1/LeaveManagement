using Reqnroll.BoDi;

namespace LeaveManagement.API.IntegrationTests.Support;

[Reqnroll.Binding]
internal class Hooks
{
    private static CustomWebApplicationFactory<Program>? _factory;

    [Reqnroll.BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        await _factory.InitializeAsync();
    }

    [Reqnroll.AfterTestRun]
    public static async Task AfterTestRun()
    {
        await _factory.DisposeAsync();
    }

    [Reqnroll.BeforeScenario(Order = 0)]
    public void RegisterFactory(IObjectContainer objectContainer)
    {
        objectContainer.RegisterInstanceAs(_factory);
    }
}

