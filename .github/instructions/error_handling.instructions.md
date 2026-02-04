---
description: 'Guidelines for robust error handling and exception management in C#'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **USE** exceptions for error handling, not error codes.
> - **THROW** specific exception types, not generic `Exception`.
> - **AVOID** empty catch blocks or swallowing exceptions.
> - **USE** `ArgumentNullException.ThrowIfNull()` for null checks (C# 11+).
> - **PREFER** built-in exception types over custom ones when appropriate.
> - **DO NOT** use exceptions for control flow.

# Error Handling

Error handling is not an afterthought—it's a critical part of writing robust, maintainable code. This chapter covers best practices for using exceptions, avoiding common pitfalls, and writing code that fails gracefully and informatively.

##### Key Principles

- **Use exceptions, not error codes**: Exceptions provide better context, stack traces, and force callers to handle errors.
- **Provide context with exceptions**: Include meaningful error messages that help diagnose the problem.
- **Don't return null**: Use exceptions, `Option<T>`, or nullable reference types to handle absence of values.
- **Define exception classes with context**: Custom exceptions should provide additional context when built-in types are insufficient.
- **Don't pass null**: Validate inputs at boundaries and use non-nullable types.
- **Extract try/catch blocks**: Keep error handling separate from business logic.
- **Fail fast**: Detect and report errors as early as possible.

##### C#/.NET Examples

Example 1: Using exceptions instead of error codes

**Poor design**

```csharp
public int DeleteUser(int userId)
{
    if (userId <= 0)
        return -1; // Error: invalid ID

    var user = _repository.FindById(userId);
    if (user == null)
        return -2; // Error: not found

    _repository.Delete(user);
    return 0; // Success
}

// Caller must remember to check
var result = DeleteUser(123);
if (result != 0)
{
    // Handle error... but what does -2 mean?
}
```

**Clean design**

```csharp
public void DeleteUser(int userId)
{
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

    var user = _repository.FindById(userId);
    if (user == null)
        throw new UserNotFoundException($"User with ID {userId} not found");

    _repository.Delete(user);
}

// Caller can use try/catch or let exception propagate
try
{
    DeleteUser(123);
}
catch (UserNotFoundException ex)
{
    // Clear exception type and message
    _logger.LogWarning(ex, "User not found");
}
```

**Comment:**
Exceptions provide clear error types, messages, and stack traces. Error codes are cryptic and easy to ignore.

---

Example 2: Avoiding null returns

**Poor design**

```csharp
public User FindUser(int id)
{
    var user = _repository.FindById(id);
    return user; // Returns null if not found
}

// Caller must remember null check
var user = FindUser(123);
if (user != null)
{
    user.UpdateEmail("new@example.com");
}
```

**Clean design**

```csharp
// Option 1: Throw exception
public User GetUser(int id)
{
    var user = _repository.FindById(id);
    return user ?? throw new UserNotFoundException($"User {id} not found");
}

// Option 2: Use nullable reference type (C# 8+)
public User? FindUser(int id)
{
    return _repository.FindById(id); // Compiler warns if caller doesn't check
}

// Option 3: Return Result<T> pattern
public Result<User> TryFindUser(int id)
{
    var user = _repository.FindById(id);
    return user != null
        ? Result<User>.Success(user)
        : Result<User>.Failure("User not found");
}
```

**Comment:**
Make nullability explicit. Use exceptions when absence is exceptional, nullable types when it's expected.

---

Example 3: Providing context in exceptions

**Poor design**

```csharp
public void ProcessPayment(Order order)
{
    if (order.Total <= 0)
        throw new Exception("Invalid order");

    if (string.IsNullOrEmpty(order.PaymentMethod))
        throw new Exception("Invalid order");

    // Process payment...
}
```

**Clean design**

```csharp
public void ProcessPayment(Order order)
{
    ArgumentNullException.ThrowIfNull(order);

    if (order.Total <= 0)
        throw new InvalidOperationException(
            $"Order {order.Id} has invalid total: {order.Total}");

    if (string.IsNullOrEmpty(order.PaymentMethod))
        throw new InvalidOperationException(
            $"Order {order.Id} is missing payment method");

    // Process payment...
}
```

**Comment:**
Specific exception types and detailed messages make debugging easier. Include relevant context like IDs and values.

---

Example 4: Separating error handling from business logic

**Poor design**

```csharp
public async Task<Order> ProcessOrderAsync(int orderId)
{
    try
    {
        var order = await _repository.GetOrderAsync(orderId);

        if (order == null)
            throw new OrderNotFoundException($"Order {orderId} not found");

        try
        {
            await ValidateOrderAsync(order);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation failed");
            throw;
        }

        try
        {
            await _paymentService.ProcessPaymentAsync(order);
        }
        catch (PaymentException ex)
        {
            _logger.LogError(ex, "Payment failed");
            throw;
        }

        return order;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing order");
        throw;
    }
}
```

**Clean design**

```csharp
public async Task<Order> ProcessOrderAsync(int orderId)
{
    var order = await GetOrderOrThrowAsync(orderId);
    await ValidateOrderAsync(order);
    await ProcessPaymentAsync(order);
    return order;
}

private async Task<Order> GetOrderOrThrowAsync(int orderId)
{
    var order = await _repository.GetOrderAsync(orderId);
    return order ?? throw new OrderNotFoundException($"Order {orderId} not found");
}

private async Task ValidateOrderAsync(Order order)
{
    try
    {
        await _validator.ValidateAsync(order);
    }
    catch (ValidationException ex)
    {
        _logger.LogWarning(ex, "Order {OrderId} validation failed", order.Id);
        throw;
    }
}

private async Task ProcessPaymentAsync(Order order)
{
    try
    {
        await _paymentService.ProcessPaymentAsync(order);
    }
    catch (PaymentException ex)
    {
        _logger.LogError(ex, "Payment failed for order {OrderId}", order.Id);
        throw;
    }
}
```

**Comment:**
Extract error handling into separate methods. The main method reads like business logic, not error handling.

---

Example 5: Using custom exceptions appropriately

**Poor design**

```csharp
// Too many custom exceptions
public class InvalidUserException : Exception { }
public class InvalidEmailException : Exception { }
public class InvalidPasswordException : Exception { }
public class UserTooYoungException : Exception { }
// ... 50 more custom exceptions
```

**Clean design**

```csharp
// Use built-in exceptions when possible
public void ValidateUser(User user)
{
    ArgumentNullException.ThrowIfNull(user);

    if (string.IsNullOrEmpty(user.Email))
        throw new ArgumentException("Email cannot be empty", nameof(user.Email));

    if (!IsValidEmail(user.Email))
        throw new FormatException($"Invalid email format: {user.Email}");
}

// Create custom exceptions only when they add value
public class InsufficientFundsException : Exception
{
    public decimal RequiredAmount { get; }
    public decimal AvailableAmount { get; }

    public InsufficientFundsException(decimal required, decimal available)
        : base($"Insufficient funds. Required: {required}, Available: {available}")
    {
        RequiredAmount = required;
        AvailableAmount = available;
    }
}
```

**Comment:**
Use built-in exception types when they fit. Create custom exceptions only when you need additional context or behavior.

## Validation and Verification

### Automated Checks
- [ ] No empty catch blocks
- [ ] No `catch (Exception)` without rethrowing
- [ ] `ArgumentNullException.ThrowIfNull()` used for null checks
- [ ] No methods returning null for collections (return empty collection instead)
- [ ] Nullable reference types enabled and used correctly

### Manual Review
- [ ] Exception messages provide actionable context
- [ ] Exceptions are specific, not generic `Exception`
- [ ] Error handling is separated from business logic
- [ ] Exceptions are not used for normal control flow
- [ ] Custom exceptions add genuine value

## Best Practices to Remember

- Use exceptions for exceptional conditions, not expected scenarios
- Fail fast—validate inputs at boundaries and throw early
- Provide clear, actionable error messages with context
- Don't swallow exceptions—log and rethrow or let them propagate
- Prefer built-in exception types; create custom ones sparingly
- Enable nullable reference types to catch null-related errors at compile time
