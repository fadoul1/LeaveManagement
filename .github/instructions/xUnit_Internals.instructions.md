---
description: 'Guidelines explains how to write better and design tests in C# following Clean Code principles'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **TESTS MUST** be independent.
> - **USE** the Arrange-Act-Assert pattern.
> - **SEPARATE** test construction from assertion.
> - **USE** `[Theory]` for parameterized tests to reduce duplication.
> - **USE** expression-bodied members for simple tests.

# xUnit Internals

This chapter explores the internals of a testing framework, originally JUnit, but adapted here for xUnit in the .NET ecosystem. Understanding how a testing framework works helps developers write better tests, design reusable components, and appreciate the principles behind automated testing.

#### Key Principles

- **Framework design matters**: A well-structured testing framework promotes clean, maintainable tests.
- **Separation of concerns**: Test execution, reporting, and assertions should be clearly separated.
- **Extensibility**: Frameworks should allow custom behaviors without breaking core functionality.
- **Readability of tests**: Tests should be expressive and easy to understand.

---
##### C#/.NET Examples with xUnit

Example 1: Basic xUnit test structure

```csharp
using Xunit;

namespace MyApp.Tests; // File-scoped namespace

public class CalculatorTests
{

    [Fact]
    public void Add_ShouldReturnSum_WhenGivenTwoNumbers()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(2, 3);

        // Assert
        Assert.Equal(5, result);
    }
}
```

**Comment:**
xUnit uses `[Fact]` for simple tests and `[Theory]` for parameterized tests. The Arrange-Act-Assert pattern improves clarity.

Example 2: Parameterized tests with `[Theory]` and `[InlineData]`

```csharp
using Xunit;

public class CalculatorTests
{
    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(-1, 4, 3)]
    [InlineData(0, 0, 0)]
    public void Add_ShouldReturnCorrectSum(int a, int b, int expected)
    {
        var calculator = new Calculator();
        
        var result = calculator.Add(a, b);

        Assert.Equal(expected, result);
    }
}
```

**Comment:**
Parameterized tests reduce duplication and improve coverage.

Example 3: Custom assertion for better readability

```csharp
public static class CustomAssertions
{
    public static void ShouldBePositive(this int value)
    {
        Assert.True(value > 0, $"Expected a positive number, but got {value}.");
    }
}
  
// Usage in test
[Fact]
public void Result_ShouldBePositive()
{
    var result = new Calculator().Add(2, 3);

    result.ShouldBePositive();
}

```

**Comment:**
Custom assertions make tests more expressive and reusable.

---

Example 4: Test fixtures with `IClassFixture`

```csharp
public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        // Setup: Create test database
        Connection = new SqlConnection("...");
        Connection.Open();
    }

    public SqlConnection Connection { get; }

    public void Dispose()
    {
        // Cleanup: Close connection
        Connection?.Dispose();
    }
}

public class DatabaseTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public DatabaseTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void SaveUser_ShouldPersistToDatabase()
    {
        // Arrange
        var repository = new UserRepository(_fixture.Connection);

        // Act & Assert
        // Test using shared fixture
    }
}
```

**Comment:**
Class fixtures allow sharing expensive setup/teardown across all tests in a class.

---

Example 5: Async tests

```csharp
[Fact]
public async Task GetUserAsync_ShouldReturnUser_WhenUserExists()
{
    // Arrange
    var repository = new UserRepository();
    var userId = 123;

    // Act
    var user = await repository.GetUserAsync(userId);

    // Assert
    Assert.NotNull(user);
    Assert.Equal(userId, user.Id);
}

[Theory]
[InlineData(1)]
[InlineData(2)]
[InlineData(3)]
public async Task LoadOrderAsync_ShouldReturnOrder_ForValidId(int orderId)
{
    var order = await _orderService.LoadOrderAsync(orderId);

    Assert.NotNull(order);
}
```

**Comment:**
xUnit natively supports async tests. Simply return `Task` and use `async/await`.

## Validation and Verification

### Automated Checks
- [ ] All tests use Arrange-Act-Assert pattern
- [ ] Tests are independent (no shared mutable state)
- [ ] `[Theory]` used for parameterized tests instead of duplicating test methods
- [ ] Async tests properly use `async/await`
- [ ] Test names follow convention: `MethodName_ShouldExpectedBehavior_WhenCondition`

### Manual Review
- [ ] Tests are readable and self-explanatory
- [ ] Tests focus on behavior, not implementation details
- [ ] Fixtures used appropriately for expensive setup
- [ ] Custom assertions used for domain-specific validations
- [ ] No logic in tests (no conditionals, loops, etc.)

## Best Practices to Remember

- Keep tests clean and readable.
- Use custom assertions for domain-specific clarity.
- Avoid logic in tests—tests should be simple and deterministic.
- Understand the framework to leverage advanced features (e.g., fixtures, custom attributes).