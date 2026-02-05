# Code Review Skill

| Property | Value |
|----------|-------|
| Name | code-review |
| Description | Expert code reviewer for C# 14/.NET 10 applications following Clean Architecture and CQRS patterns |

## Overview

This skill performs thorough, constructive, and actionable code reviews that help developers improve code quality, maintainability, and adherence to best practices. It specializes in C# 14/.NET 10 applications following Clean Architecture and CQRS patterns.

## When to Use

- Code changes need review before merging
- Pull request requires quality assessment
- New feature implementation needs validation
- Refactoring changes need verification
- User requests code review or feedback
- Code quality issues need identification

## Review Principles

### The Golden Rules

1. **Be Constructive**: Provide solutions, not just criticism
2. **Be Specific**: Reference exact lines and files
3. **Explain Why**: Help developers understand the reasoning
4. **Prioritize Issues**: Critical issues first, minor suggestions last
5. **Acknowledge Good Practices**: Recognize well-written code

### What to Review

- Architecture and design patterns
- SOLID principles compliance
- Modern C# 14/.NET 10 syntax usage
- Security vulnerabilities
- Performance concerns
- Test coverage and quality

### What NOT to Review

- Personal style preferences without objective benefit
- Working code that meets requirements without issues
- Minor formatting when auto-formatters exist
- Historical code not part of the current changes

## Review Categories

### 1. Architecture & Design

Check for Clean Architecture layer separation and CQRS compliance.

**Issues to Detect:**

```csharp
// BAD: Business logic in controller
[HttpPost]
public async Task<ActionResult> Create([FromBody] CreateLeaveCommand command)
{
    if (command.StartDate > command.EndDate)
        return BadRequest("Invalid dates");

    var leave = new Leave { StartDate = command.StartDate };
    await _context.Leaves.AddAsync(leave);
    return Ok();
}

// GOOD: Controller delegates to MediatR
[HttpPost]
public async Task<ActionResult<LeaveResponse>> Create([FromBody] CreateLeaveCommand command)
{
    var response = await mediator.Send(command);
    return response switch
    {
        { Success: true } => Ok(response),
        _ => BadRequest(response)
    };
}
```

### 2. C# 14/.NET 10 Modern Syntax

Identify opportunities to use modern language features.

**Pattern 1: Traditional to Primary Constructor**

```csharp
// BEFORE
public class CreateLeaveHandler : IRequestHandler<CreateLeaveCommand, LeaveResponse?>
{
    private readonly ILeaveRepository _repository;
    private readonly IValidator<CreateLeaveCommand> _validator;

    public CreateLeaveHandler(ILeaveRepository repository, IValidator<CreateLeaveCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }
}

// AFTER (C# 14)
public class CreateLeaveHandler(
    ILeaveRepository repository,
    IValidator<CreateLeaveCommand> validator) : IRequestHandler<CreateLeaveCommand, LeaveResponse?>
{
}
```

**Pattern 2: Class to Record for DTOs**

```csharp
// BEFORE
public class LeaveResponse
{
    public int Id { get; set; }
    public string EmployeeName { get; set; }
    public bool Success { get; set; }
}

// AFTER (C# 14)
public record LeaveResponse
{
    public int Id { get; init; }
    public required string EmployeeName { get; init; }
    public bool Success { get; init; }
}
```

**Pattern 3: If-Else to Switch Expression**

```csharp
// BEFORE
if (response != null && response.Success)
    return Ok(response);
else if (response != null)
    return BadRequest(response);
else
    return NotFound();

// AFTER (C# 14)
return response switch
{
    { Success: true } => Ok(response),
    not null => BadRequest(response),
    _ => NotFound()
};
```

**Pattern 4: Traditional Extension to Extension Type**

```csharp
// BEFORE
public static class LeaveMapper
{
    public static Leave ToLeave(this CreateLeaveCommand command) => new()
    {
        StartDate = command.StartDate,
        EndDate = command.EndDate,
    };
}

// AFTER (C# 14)
public implicit extension CreateLeaveCommandExtensions for CreateLeaveCommand
{
    public Leave ToLeave() => new()
    {
        StartDate = StartDate,
        EndDate = EndDate,
    };
}
```

**Pattern 5: Null Check to ThrowIfNull**

```csharp
// BEFORE
if (entity == null)
    throw new ArgumentNullException(nameof(entity));

// AFTER (C# 14)
ArgumentNullException.ThrowIfNull(entity);
```

### 3. Code Quality

**Issues to Detect:**

| Issue | Detection |
|-------|-----------|
| Magic numbers/strings | Hardcoded values without constants |
| Code duplication | Similar code blocks in multiple places |
| Long methods | Methods exceeding 20 lines |
| Too many parameters | More than 3 parameters |
| High complexity | Nested conditions, multiple branches |

```csharp
// BAD: Magic numbers and long method
public decimal CalculateLeaveBalance(Employee employee)
{
    var balance = 25; // Magic number
    if (employee.YearsOfService > 5)
        balance += 5;
    if (employee.YearsOfService > 10)
        balance += 5;
    // ... 30 more lines
}

// GOOD: Named constants and focused method
private const int BaseLeaveAllowance = 25;
private const int SeniorBonusDays = 5;
private const int VeteranBonusDays = 5;

public decimal CalculateLeaveBalance(Employee employee)
    => BaseLeaveAllowance + CalculateSeniorityBonus(employee.YearsOfService);

private int CalculateSeniorityBonus(int yearsOfService) => yearsOfService switch
{
    > 10 => SeniorBonusDays + VeteranBonusDays,
    > 5 => SeniorBonusDays,
    _ => 0
};
```

### 4. Naming Conventions

**Rules:**

| Type | Convention | Example |
|------|------------|---------|
| Boolean properties | Is/Has/Can/Should prefix | `IsApproved`, `HasAccess` |
| Async methods | Async suffix | `GetLeaveAsync` |
| Collections | Plural names | `Employees`, `LeaveRequests` |
| Constants | PascalCase | `MaxRetryCount` |

```csharp
// BAD
public bool approved { get; set; }
public async Task<Leave> GetLeave(int id);
public List<Employee> emp;

// GOOD
public bool IsApproved { get; set; }
public async Task<Leave> GetLeaveAsync(int id);
public List<Employee> Employees;
```

### 5. Error Handling

**Issues to Detect:**

```csharp
// BAD: Empty catch block
try
{
    await repository.SaveAsync(entity);
}
catch (Exception)
{
    // Swallowed exception
}

// BAD: Generic exception
throw new Exception("Something went wrong");

// GOOD: Specific exception with context
try
{
    await repository.SaveAsync(entity);
}
catch (DbUpdateException ex)
{
    logger.LogError(ex, "Failed to save leave request {LeaveId}", entity.Id);
    throw new LeaveManagementException($"Failed to save leave request {entity.Id}", ex);
}
```

### 6. Security

**Issues to Detect:**

- SQL injection vulnerabilities
- XSS vulnerabilities
- Hardcoded credentials
- Missing authorization checks
- Sensitive data in logs

```csharp
// BAD: SQL injection risk
var query = $"SELECT * FROM Leaves WHERE EmployeeId = {employeeId}";

// GOOD: Parameterized query
var leaves = await context.Leaves
    .Where(l => l.EmployeeId == employeeId)
    .ToListAsync();
```

### 7. Performance

**Issues to Detect:**

| Issue | Detection |
|-------|-----------|
| N+1 queries | Queries in loops |
| Blocking async | `.Result` or `.Wait()` |
| Missing cancellation | No CancellationToken |
| Unnecessary allocations | LINQ ToList() when IEnumerable works |

```csharp
// BAD: N+1 query
foreach (var leave in leaves)
{
    leave.Employee = await context.Employees.FindAsync(leave.EmployeeId);
}

// GOOD: Eager loading
var leaves = await context.Leaves
    .Include(l => l.Employee)
    .ToListAsync(cancellationToken);
```

### 8. Testing

**Rules:**

- AAA pattern (Arrange-Act-Assert)
- One assertion per test concept
- Naming: `Method_Should_When`
- Use `[Theory]` for parameterized tests
- Use Shouldly assertions

```csharp
// BAD: Multiple concepts, poor naming
[Fact]
public async Task Test1()
{
    var result = await sut.Handle(command, CancellationToken.None);
    Assert.NotNull(result);
    Assert.True(result.Success);
    Assert.Equal(1, result.Id);
    Assert.Equal("Test", result.Name);
}

// GOOD: Single concept, clear naming
[Fact]
public async Task Handle_ShouldReturnSuccess_WhenValidCommand()
{
    // Arrange
    var command = new CreateLeaveCommand(StartDate: today, EndDate: tomorrow);

    // Act
    var result = await sut.Handle(command, CancellationToken.None);

    // Assert
    result.ShouldNotBeNull();
    result.Success.ShouldBeTrue();
}

[Theory]
[InlineData("")]
[InlineData(" ")]
[InlineData(null)]
public async Task Handle_ShouldFail_WhenNameIsInvalid(string? invalidName)
{
    // Arrange
    var command = new CreateLeaveCommand(Name: invalidName!);

    // Act
    var result = await sut.Handle(command, CancellationToken.None);

    // Assert
    result!.Success.ShouldBeFalse();
}
```

## Review Output Format

### Summary Section

```markdown
## Code Review Summary

**Files Reviewed:** X files
**Severity:** Critical | Major | Minor | Info

### Overview
[Brief description of the changes and their purpose]

### Verdict
- [ ] Approved
- [ ] Approved with suggestions
- [ ] Changes requested
```

### Issue Format

```markdown
#### [Category] Issue Title

**File:** `path/to/file.cs`
**Line:** 42-45
**Severity:** Critical | Major | Minor | Info
**Rule:** [instruction_file.md] or [principle]

**Problem:**
[Description of the issue]

**Current Code:**
```csharp
// problematic code
```

**Suggested Fix:**
```csharp
// improved code
```

**Why:**
[Explanation of why this change improves the code]
```

## Severity Definitions

| Severity | Definition | Action Required |
|----------|------------|-----------------|
| **Critical** | Security vulnerability, data loss risk, crash potential | Must fix before merge |
| **Major** | Architecture violation, significant code smell, missing tests | Should fix before merge |
| **Minor** | Style inconsistency, naming issue, optimization opportunity | Consider fixing |
| **Info** | Suggestion, learning opportunity, alternative approach | Optional |

## Instruction Files Reference

This skill reviews code against the following instruction files:

| File | Key Checks |
|------|------------|
| `classes.instructions.md` | SRP, max 3-4 dependencies, records for immutable data |
| `functions.instructions.md` | Single responsibility, max 3 arguments, expression-bodied members |
| `meaningful_names.instructions.md` | Intent-revealing names, Is/Has/Can/Should for booleans |
| `comments.instructions.md` | No redundant comments, no commented-out code |
| `formatting.instructions.md` | File-scoped namespaces, named arguments |
| `error_handling.instructions.md` | ArgumentNullException.ThrowIfNull(), specific exceptions |
| `unit_tests.instructions.md` | AAA pattern, one assertion per test |
| `xUnit_Internals.instructions.md` | [Theory] for parameterized tests, independent tests |
| `objects_and_data_structures.instructions.md` | Hide data, expose behaviors, Law of Demeter |
| `emergence.instructions.md` | DRY, switch expressions, pattern matching |
| `boundaries.instructions.md` | Encapsulate third-party libs, translate exceptions |

## Review Checklist

### Architecture & Design
- [ ] Follows Clean Architecture layer separation
- [ ] Respects CQRS pattern (Commands vs Queries)
- [ ] Single Responsibility Principle respected
- [ ] No business logic in controllers
- [ ] No infrastructure concerns in domain

### C# 14/.NET 10 Syntax
- [ ] File-scoped namespaces used
- [ ] Primary constructors for DI
- [ ] Records for Commands, Queries, DTOs
- [ ] Pattern matching and switch expressions
- [ ] Collection expressions where appropriate

### Code Quality
- [ ] No magic numbers or strings
- [ ] No code duplication (DRY)
- [ ] Methods have single responsibility
- [ ] Max 3 parameters per method
- [ ] Max 20 lines per method

### Security
- [ ] No SQL injection vulnerabilities
- [ ] Input validation at boundaries
- [ ] No hardcoded credentials
- [ ] Proper authorization checks

### Performance
- [ ] No N+1 query problems
- [ ] Async/await used correctly
- [ ] No .Result or .Wait() on Tasks
- [ ] Proper use of cancellation tokens

### Testing
- [ ] Unit tests follow AAA pattern
- [ ] One assertion per test concept
- [ ] Test names: Method_Should_When
- [ ] [Theory] for parameterized tests
