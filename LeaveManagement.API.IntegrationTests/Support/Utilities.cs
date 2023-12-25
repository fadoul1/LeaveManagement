using LeaveManagement.Domain.Entities;
using LeaveManagement.Persistence.Data;

namespace LeaveManagement.API.IntegrationTests.Support;

internal static class Utilities
{
    public static void InitializeEmployeesForTests(ApplicationContext context)
    {
        List<Employee>? initialEmployees =
        [
            new() 
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123456789"
            },
            new() 
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                PhoneNumber = "987654321"
            }
        ];

        context.Employees.AddRange(initialEmployees);
        context.SaveChanges();
    }
}