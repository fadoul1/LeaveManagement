using LeaveManagement.API.IntegrationTests.Support;
using LeaveManagement.Application.Features.Leaves.Commands.CreateLeave;
using LeaveManagement.Application.Responses;
using LeaveManagement.Tests.Common;
using Newtonsoft.Json;
using System.Text;
using TechTalk.SpecFlow.Assist;

namespace LeaveManagement.API.IntegrationTests.StepDefinitions;

[Binding]
internal class LeaveApiStepDefinitions
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;
    private HttpResponseMessage _response = new();

    public LeaveApiStepDefinitions(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Given(@"there are existing leaves")]
    public void GivenThereAreExistingLeaves()
    {
        // Database seeding is handled by DatabaseHook
        // This step exists for Gherkin readability and to explicitly document the precondition
    }

    [When(@"the user requests the list of leaves")]
    public async Task WhenTheUserRequestsTheListOfLeaves()
    {
        _response = await _httpClient.GetAsync(Routes.LeaveRessource);
    }

    [Then(@"the response should contain a list of leaves")]
    public async Task ThenTheResponseShouldContainAListOfLeaves()
    {
        _response.EnsureSuccessStatusCode();
        var content = await _response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<List<LeaveResponse>>(content);

        responseObject.Should().NotBeNull();
        responseObject!.Should().NotBeEmpty();
    }

    [When(@"the user requests the leaves for employee with ID (.*)")]
    public async Task WhenTheUserRequestsTheLeavesForEmployeeWithId(long employeeId)
    {
        _response = await _httpClient.GetAsync($"{Routes.LeaveRessource}/{employeeId}");
    }

    [Then(@"the response should contain leaves for employee with ID (.*)")]
    public async Task ThenTheResponseShouldContainLeavesForEmployeeWithId(long employeeId)
    {
        _response.EnsureSuccessStatusCode();
        var content = await _response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<List<LeaveResponse>>(content);

        responseObject.Should().NotBeNull();
        responseObject!.Should().OnlyContain(leave => leave.EmployeeId == employeeId);
    }

    [When(@"the user creates a new leave with the following details:")]
    public async Task WhenTheUserCreatesANewLeaveWithTheFollowingDetails(Table table)
    {
        var createLeaveCommand = table.CreateInstance<CreateLeaveCommand>();
        var jsonContent = JsonConvert.SerializeObject(createLeaveCommand);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _response = await _httpClient.PostAsync(Routes.LeaveRessource, httpContent);
    }

    [Then(@"the response should indicate a successful leave creation")]
    public async Task ThenTheResponseShouldIndicateASuccessfulLeaveCreation()
    {
        _response.EnsureSuccessStatusCode();
        var content = await _response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<LeaveResponse>(content);

        responseObject.Should().NotBeNull();
        responseObject!.Success.Should().BeTrue();
    }
}



