---
description: 'Guidelines for writing clean, maintainable unit tests in C#'
applyTo: '**/*Tests.cs, **/*.Tests/**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **USE** the Arrange-Act-Assert (AAA) pattern in all tests.
> - **ONE** assertion per test (or closely related assertions).
> - **TESTS MUST** be independent and run in any order.
> - **USE** descriptive test names: `MethodName_ShouldExpectedBehavior_WhenCondition`.
> - **AVOID** logic in tests (no conditionals, loops, or complex setup).
> - **PREFER** test data builders over complex object construction.

# Unit Tests

Clean tests are as important as clean production code. This chapter focuses on writing tests that are readable, maintainable, and trustworthy. Good tests serve as living documentation and provide confidence when refactoring.

##### Key Principles

- **One assert per test**: Each test should verify one concept, making failures easy to diagnose.
- **F.I.R.S.T. principles**:
  - **Fast**: Tests should run quickly
  - **Independent**: Tests should not depend on each other
  - **Repeatable**: Same result every time in any environment
  - **Self-validating**: Pass or fail, no manual inspection
  - **Timely**: Write tests before or with production code (TDD)
- **Clean test code**: Tests are first-class code and deserve the same care as production code.
- **Readable tests**: Tests should read like specifications—clear and obvious.
- **Test behavior, not implementation**: Tests should verify what code does, not how it does it.
- **Arrange-Act-Assert**: Structure tests in three clear sections.

##### C#/.NET Examples

Example 1: Arrange-Act-Assert pattern

**Poor design**

```csharp
[Fact]
public void TestCalculator()
{
    var calculator = new Calculator();
    var result = calculator.Add(2, 3);
    Assert.Equal(5, result);
    var result2 = calculator.Subtract(10, 4);
    Assert.Equal(6, result2);
    var result3 = calculator.Multiply(3, 4);
    Assert.Equal(12, result3);
    // Multiple operations mixed together, unclear sections
}
```

**Clean design**

```csharp
[Fact]
public void Add_ShouldReturnSum_WhenGivenTwoPositiveNumbers()
{
    // Arrange
    var calculator = new Calculator();
    int firstNumber = 2;
    int secondNumber = 3;

    // Act
    int result = calculator.Add(firstNumber, secondNumber);

    // Assert
    Assert.Equal(5, result);
}
```

**Comment:**
AAA pattern makes tests readable. Each section has a clear purpose, and the test verifies one specific behavior.

---

Example 2: Descriptive test names

**Poor design**

```csharp
[Fact]
public void Test1() { }

[Fact]
public void TestUser() { }

[Fact]
public void ValidateEmail() { }
```

**Clean design**

```csharp
[Fact]
public void ValidateEmail_ShouldReturnTrue_WhenEmailIsValid() { }

[Fact]
public void ValidateEmail_ShouldThrowArgumentException_WhenEmailIsNull() { }

[Fact]
public void CreateUser_ShouldAssignDefaultRole_WhenRoleIsNotSpecified() { }
```

**Comment:**
Test names should describe what is being tested, the expected outcome, and the condition. They serve as documentation.

---

Example 3: One assertion per test

**Poor design**

```csharp
[Fact]
public void CreateUser_ShouldSetAllProperties()
{
    var user = CreateUser("John", "john@example.com", 30);

    Assert.Equal("John", user.Name);
    Assert.Equal("john@example.com", user.Email);
    Assert.Equal(30, user.Age);
    Assert.NotNull(user.Id);
    Assert.True(user.IsActive);
    Assert.NotNull(user.CreatedAt);
}
```

**Clean design**

```csharp
[Fact]
public void CreateUser_ShouldSetName_WhenProvided()
{
    var user = CreateUser(name: "John", email: "john@example.com", age: 30);

    Assert.Equal("John", user.Name);
}

[Fact]
public void CreateUser_ShouldSetEmail_WhenProvided()
{
    var user = CreateUser(name: "John", email: "john@example.com", age: 30);

    Assert.Equal("john@example.com", user.Email);
}

[Fact]
public void CreateUser_ShouldSetActiveStatusToTrue_ByDefault()
{
    var user = CreateUser(name: "John", email: "john@example.com", age: 30);

    Assert.True(user.IsActive);
}
```

**Comment:**
One assertion per test makes failures immediately clear. When a test fails, you know exactly what broke.

---

Example 4: Test independence

**Poor design**

```csharp
public class UserServiceTests
{
    private User _testUser;

    [Fact]
    public void Test1_CreateUser()
    {
        _testUser = _service.CreateUser("John", "john@example.com");
        Assert.NotNull(_testUser);
    }

    [Fact]
    public void Test2_UpdateUser()
    {
        // Depends on Test1 running first!
        _testUser.Name = "Jane";
        _service.Update(_testUser);
        Assert.Equal("Jane", _testUser.Name);
    }
}
```

**Clean design**

```csharp
public class UserServiceTests
{
    [Fact]
    public void CreateUser_ShouldReturnUser_WhenValidDataProvided()
    {
        var user = _service.CreateUser("John", "john@example.com");

        Assert.NotNull(user);
    }

    [Fact]
    public void UpdateUser_ShouldChangeProperties_WhenCalled()
    {
        // Arrange - each test creates its own data
        var user = _service.CreateUser("John", "john@example.com");

        // Act
        user.Name = "Jane";
        _service.Update(user);

        // Assert
        var updatedUser = _service.GetUser(user.Id);
        Assert.Equal("Jane", updatedUser.Name);
    }
}
```

**Comment:**
Each test sets up its own data and doesn't rely on other tests. Tests can run in any order.

---

Example 5: Using test data builders

**Poor design**

```csharp
[Fact]
public void ProcessOrder_ShouldCalculateTotal()
{
    var order = new Order
    {
        Id = 1,
        CustomerId = 123,
        Customer = new Customer
        {
            Id = 123,
            Name = "John",
            Email = "john@example.com",
            Address = new Address
            {
                Street = "123 Main St",
                City = "Springfield",
                State = "IL",
                ZipCode = "62701"
            }
        },
        Items = new List<OrderItem>
        {
            new OrderItem { ProductId = 1, Quantity = 2, Price = 10.00m },
            new OrderItem { ProductId = 2, Quantity = 1, Price = 20.00m }
        },
        TaxRate = 0.08m
    };

    var total = _service.CalculateTotal(order);

    Assert.Equal(43.20m, total);
}
```

**Clean design**

```csharp
// Test data builder
public class OrderBuilder
{
    private List<OrderItem> _items = new();
    private decimal _taxRate = 0.08m;

    public OrderBuilder WithItem(decimal price, int quantity = 1)
    {
        _items.Add(new OrderItem { Price = price, Quantity = quantity });
        return this;
    }

    public OrderBuilder WithTaxRate(decimal rate)
    {
        _taxRate = rate;
        return this;
    }

    public Order Build() => new()
    {
        Items = _items,
        TaxRate = _taxRate
    };
}

[Fact]
public void CalculateTotal_ShouldIncludeTax_WhenTaxRateProvided()
{
    var order = new OrderBuilder()
        .WithItem(price: 10.00m, quantity: 2)
        .WithItem(price: 20.00m)
        .WithTaxRate(0.08m)
        .Build();

    var total = _service.CalculateTotal(order);

    Assert.Equal(43.20m, total);
}
```

**Comment:**
Test data builders make test setup expressive and hide irrelevant details. Focus on what matters for each test.

---

Example 6: Testing behavior, not implementation

**Poor design**

```csharp
[Fact]
public void ProcessOrder_ShouldCallRepositorySaveMethod()
{
    var mockRepo = new Mock<IOrderRepository>();
    var service = new OrderService(mockRepo.Object);
    var order = new Order();

    service.ProcessOrder(order);

    mockRepo.Verify(r => r.Save(order), Times.Once);
    // This test knows too much about implementation
}
```

**Clean design**

```csharp
[Fact]
public void ProcessOrder_ShouldPersistOrder_WhenCalled()
{
    var order = new OrderBuilder().Build();

    _service.ProcessOrder(order);

    var savedOrder = _repository.GetById(order.Id);
    Assert.NotNull(savedOrder);
    Assert.Equal(order.Id, savedOrder.Id);
    // Tests the outcome, not how it's achieved
}
```

**Comment:**
Test what the code does (behavior), not how it does it (implementation). This makes tests resilient to refactoring.

## Validation and Verification

### Automated Checks
- [ ] All tests use Arrange-Act-Assert pattern
- [ ] Test names follow `MethodName_ShouldExpectedBehavior_WhenCondition` convention
- [ ] Tests are independent (can run in any order)
- [ ] No conditionals or loops in test code
- [ ] Tests run fast (< 100ms per test ideally)

### Manual Review
- [ ] Each test verifies one concept
- [ ] Tests are readable and self-documenting
- [ ] Test data builders used for complex object construction
- [ ] Tests focus on behavior, not implementation details
- [ ] No shared mutable state between tests

## Best Practices to Remember

- Write tests that read like specifications
- Keep tests simple—no logic in tests
- Use test data builders for complex setup
- Test one concept per test
- Make tests independent and fast
- Refactor tests with the same care as production code
- Let tests guide your design (TDD)
