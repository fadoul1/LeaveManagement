using LeaveManagement.Domain.Entities;
using LeaveManagement.Domain.Enumerations;
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
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123456789"
            },
            new() 
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                PhoneNumber = "987654321"
            }
        ];

        context.Employees.AddRange(initialEmployees);
        context.SaveChanges();
    }

    public static void InitializeLeavesForTests(ApplicationContext context)
    {
        if (!context.Employees.Any())
        {
            InitializeEmployeesForTests(context);
        }

        // Check if leaves already exist to avoid duplicate insertions during parallel tests
        if (context.Leaves.Any())
        {
            return;
        }

        // Get actual employee IDs from the database
        var employees = context.Employees
            .Where(e => e.DeletedAt == DateTimeOffset.MinValue.ToUniversalTime())
            .OrderBy(e => e.Id)
            .Take(2)
            .ToList();
            
        if (employees.Count < 2)
        {
            return; // Not enough employees to create leaves
        }

        List<Leave> initialLeaves =
        [
            new()
            {
                Type = LeaveTypeEnum.AnnualLeave,
                Status = LeaveStatusEnum.InProgress,
                StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-10), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-7), DateTimeKind.Utc),
                Reason = "Rest",
                EmployeeId = employees[0].Id
            },
            new()
            {
                Type = LeaveTypeEnum.SickLeave,
                Status = LeaveStatusEnum.Finish,
                StartDate = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-5), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-3), DateTimeKind.Utc),
                Reason = "Flu",
                EmployeeId = employees[1].Id
            }
        ];

        context.Leaves.AddRange(initialLeaves);
        context.SaveChanges();
    }
}