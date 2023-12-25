using LeaveManagement.Domain.Entities;
using LeaveManagement.Persistence.Data;

namespace LeaveManagement.Tests.IntegrationTests.Common;

internal static class Utilities
{
    public static void InitializeDbForTests(ApplicationContext context)
    {
        List<Employee>? initialEmployees =
        [
            new() {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123456789"
            },
            new() {
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