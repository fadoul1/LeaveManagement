using LeaveManagement.Domain.Contracts.Repositories;
using LeaveManagement.Domain.Entities;
using LeaveManagement.Persistence.Data;
using LeaveManagement.Persistence.Repositories.Base;
using LeaveManagement.Tests.IntegrationTests.Common;
using TechTalk.SpecFlow.Assist;

namespace LeaveManagement.Persistence.IntegrationTests.StepDefinitions;

[Binding]
public class EmployeeStepDefinitions
{

    private readonly ApplicationContext _dbContext;
    private readonly IBaseRepository<Employee> _employeeRepository;
    private List<Employee> _employees;
    private Employee _createdEmployee;

    public EmployeeStepDefinitions()
    {
        var options = TestDatabaseConfig.GetInMemoryDatabaseOptions("LeaveManagementDbInMemoryTest");
        _dbContext = new ApplicationContext(options);

        _employeeRepository = new BaseRepository<Employee>(_dbContext);
        TestDatabaseConfig.SetupInMemoryDatabase(_dbContext);
    }

    [Given(@"there are existing employees in the database")]
    public async Task GivenThereAreExistingEmployeesInTheDatabase()
    {
        var employee1 = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "123456789"
        };

        var employee2 = new Employee
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PhoneNumber = "987654321"
        };

        await _employeeRepository.CreateAsync(employee1);
        await _employeeRepository.CreateAsync(employee2);

        _employees = new List<Employee> { employee1, employee2 };
    }

    [When(@"the user queries for the list of employees")]
    public async Task WhenTheUserQueriesForTheListOfEmployees()
    {
        _employees = await _employeeRepository.GetAllAsync();
    }

    [Then(@"the system should return a list of employees from the database")]
    public void ThenTheSystemShouldReturnAListOfEmployeesFromTheDatabase()
    {
        _employees.Should().NotBeNull().And.NotBeEmpty();
    }

    [Given(@"there is an employee with ID (.*) in the database")]
    public void GivenThereIsAnEmployeeWithIDInTheDatabase(int employeeId)
    {
        _employees.Add(new Employee
        {
            Id = employeeId,
            FirstName = "Existing",
            LastName = "Employee",
            Email = $"existing.employee{employeeId}@example.com"
        });
    }

    [When(@"the user queries for the employee with ID (.*)")]
    public async Task WhenTheUserQueriesForTheEmployeeWithID(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        _employees.Clear();
        _employees.Add(employee);
    }

    [Then(@"the system should return the employee with ID (.*) from the database")]
    public void ThenTheSystemShouldReturnTheEmployeeWithIDFromTheDatabase(int employeeId)
    {
        var retrievedEmployee = _employees.FirstOrDefault();
        retrievedEmployee.Should().NotBeNull();
        retrievedEmployee!.Id.Should().Be(employeeId);
    }

    [When(@"the user creates a new employee with details:")]
    public async Task WhenTheUserCreatesANewEmployeeWithDetails(Table table)
    {
        var employeeDetails = table.CreateInstance<Employee>();
        var newEmployee = new Employee
        {
            FirstName = employeeDetails.FirstName,
            LastName = employeeDetails.LastName,
            Email = employeeDetails.Email
        };

        _createdEmployee = await _employeeRepository.CreateAsync(newEmployee);
    }

    [Then(@"the system should successfully create the employee")]
    public void ThenTheSystemShouldSuccessfullyCreateTheEmployee()
    {
        _createdEmployee.Should().NotBeNull();
    }

    [Given(@"there is an existing employee with ID (.*) in the database")]
    public void GivenThereIsAnExistingEmployeeWithIDInTheDatabase(int employeeId)
    {
        _employees.Add(new Employee
        {
            Id = employeeId,
            FirstName = "Existing",
            LastName = "Employee",
            Email = $"existing.employee{employeeId}@example.com"
        });
    }

    [When(@"the user updates the employee with ID (.*) with details:")]
    public async Task WhenTheUserUpdatesTheEmployeeWithIDWithDetails(int employeeId, Table table)
    {
        var employeeDetails = table.CreateInstance<Employee>();
        var existingEmployee = _employees.Find(e => e.Id == employeeId);

        if (existingEmployee != null)
        {
            existingEmployee.FirstName = employeeDetails.FirstName;
            existingEmployee.LastName = employeeDetails.LastName;
            existingEmployee.Email = employeeDetails.Email;

            await _employeeRepository.UpdateAsync(existingEmployee);
        }
    }

    [Then(@"the system should successfully update the employee with ID (.*)")]
    public void ThenTheSystemShouldSuccessfullyUpdateTheEmployeeWithID(int employeeId)
    {
        var updatedEmployee = _employees.Find(e => e.Id == employeeId);
        updatedEmployee.Should().NotBeNull();
    }

    [Given(@"there is an employee with ID (.*) in the database we want to delete")]
    public async Task GivenThereIsAnEmployeeWithIDInTheDatabaseWeWantToDelete(int employeeId)
    {
        await _employeeRepository.CreateAsync(new Employee
        {
            Id = employeeId,
            FirstName = "Existing",
            LastName = "Employee",
            Email = $"existing.employee{employeeId}@example.com"
        });
    }


    [When(@"the user deletes the employee with ID (.*)")]
    public async Task WhenTheUserDeletesTheEmployeeWithID(int employeeId)
    {
        var existingEmployee = await _employeeRepository.GetByIdAsync(employeeId);

        if (existingEmployee != null)
        {
            await _employeeRepository.DeleteAsync(employeeId);
        }
    }

    [Then(@"the system should successfully delete the employee with ID (.*)")]
    public async Task ThenTheSystemShouldSuccessfullyDeleteTheEmployeeWithID(int employeeId)
    {
        var deletedEmployee = await _employeeRepository.GetByIdAsync(employeeId);
        deletedEmployee.DeletedAt.Should().NotBe(DateTime.MinValue);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _dbContext.Database.EnsureDeleted();
    }
}
