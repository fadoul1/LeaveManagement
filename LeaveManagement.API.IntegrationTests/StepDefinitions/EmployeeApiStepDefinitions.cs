using LeaveManagement.API.IntegrationTests.Support;
using LeaveManagement.Application.Features.Employees.Commands.CreateEmployee;
using LeaveManagement.Application.Features.Employees.Commands.UpdateEmployee;
using LeaveManagement.Application.Responses;
using LeaveManagement.Persistence.Data;
using LeaveManagement.Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;
using TechTalk.SpecFlow.Assist;

namespace LeaveManagement.API.IntegrationTests.StepDefinitions;

[Binding]
internal class EmployeeApiStepDefinitions
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;
    private HttpResponseMessage _response;

    public EmployeeApiStepDefinitions(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();       
    }

    [Given(@"the employee API is configured for testing")]
    public void GivenTheEmployeeAPIIsConfiguredForTesting()
    {
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationContext>();

        Utilities.InitializeEmployeesForTests(dbContext);
    }

    [When(@"the user requests the list of employees")]
    public async Task WhenTheUserRequestsTheListOfEmployees()
    {
        _response = await _httpClient.GetAsync(Routes.EmployeeRessource);
    }

    [Then(@"the response should contain a list of employees")]
    public async Task ThenTheResponseShouldContainAListOfEmployees()
    {
        _response.EnsureSuccessStatusCode();
        var content = await _response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<List<EmployeeResponse>>(content);

        content.Should().NotBeNullOrEmpty();
        responseObject.Should().NotBeEmpty();
        responseObject.Should().HaveCount(2);
    }

    [Given(@"there is an existing employee with ID (.*)")]
    public async Task GivenThereIsAnExistingEmployeeWithID(long employeeId)
    {
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationContext>();

        var employee = await dbContext.Employees.FindAsync(employeeId);
    }

    [When(@"the user requests the employee with ID (.*)")]
    public async Task WhenTheUserRequestsTheEmployeeWithID(long employeeId)
    {
        _response = await _httpClient.GetAsync($"{Routes.EmployeeRessource}/{employeeId}");
    }

    [Then(@"the response should contain the employee with ID (.*)")]
    public async Task ThenTheResponseShouldContainTheEmployeeWithID(long employeeId)
    {
        _response.EnsureSuccessStatusCode();
        var content = await _response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<EmployeeResponse>(content);

        responseObject.Should().NotBeNull();
        responseObject.EmployeeId.Should().Be(employeeId);
    }

    [When(@"the user creates a new employee with the following details:")]
    public async Task WhenTheUserCreatesANewEmployeeWithTheFollowingDetails(Table table)
    {
        var createEmployeeCommand = table.CreateInstance<CreateEmployeeCommand>();
        var jsonContent = JsonConvert.SerializeObject(createEmployeeCommand);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _response = await _httpClient.PostAsync(Routes.EmployeeRessource, httpContent);
    }

    [Then(@"the response should indicate a successful creation")]
    public async Task ThenTheResponseShouldIndicateASuccessfulCreation()
    {
        _response.EnsureSuccessStatusCode();
        var content = await _response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<EmployeeResponse>(content);

        responseObject.Should().NotBeNull();
        _response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        responseObject.Success.Should().BeTrue();
    }

    [When(@"the user updates the employee with ID (.*) with the following details:")]
    public async Task WhenTheUserUpdatesTheEmployeeWithIDWithTheFollowingDetails(long employeeId, Table table)
    {
        var createEmployeeCommand = table.CreateInstance<UpdateEmployeeCommand>();
        createEmployeeCommand.EmployeeId = employeeId;

        var jsonContent = JsonConvert.SerializeObject(createEmployeeCommand);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _response = await _httpClient.PutAsync(Routes.EmployeeRessource, httpContent);
    }

    [Then(@"the response should indicate a successful update")]
    public async Task ThenTheResponseShouldIndicateASuccessfulUpdate()
    {
        _response.EnsureSuccessStatusCode();
        var content = await _response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<EmployeeResponse>(content);

        responseObject.Should().NotBeNull();
        _response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        responseObject.Success.Should().BeTrue();
        responseObject.FirstName.Should().Be("Updated");
    }

    [When(@"the user deletes the employee with ID (.*)")]
    public async Task WhenTheUserDeletesTheEmployeeWithID(long employeeId)
    {    
        _response = await _httpClient.DeleteAsync($"{Routes.EmployeeRessource}/{employeeId}");
    }

    [Then(@"the response should indicate a successful deletion")]
    public void ThenTheResponseShouldIndicateASuccessfulDeletion()
    {
        _response.EnsureSuccessStatusCode();
        _response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }    
}
