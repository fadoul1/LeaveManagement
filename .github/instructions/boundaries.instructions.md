---
description: 'Guidelines for managing boundaries between your C# code and external code (libraries, frameworks, APIs)'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **MUST** encapsulate third-party libraries behind interfaces or adapter classes.
> - **MUST** use dependency injection to provide external dependencies.
> - **MUST NOT** let external exceptions propagate directly through your domain code.
> - **SHOULD** write learning tests to understand third-party library behavior.
> - **PREFER** built-in .NET abstractions over custom wrappers when available.
> - **AVOID** direct calls to external APIs scattered throughout the codebase.
> - **USE** fakes or mocks for external dependencies in unit tests.

# Boundaries

Boundaries are the places where your code meets external code—third-party libraries, frameworks, databases, web services, or any system you don't control. Managing these boundaries well is crucial for maintainability, testability, and adaptability. When external code changes (and it will), you want those changes isolated to a small part of your system.

This chapter teaches you how to protect your codebase from external volatility by wrapping third-party code in abstractions you control. By applying the Dependency Inversion Principle at boundaries, you create a flexible architecture that can adapt when the outside world inevitably changes.

##### Key Principles

- **Encapsulate external code**: Wrap third-party libraries behind interfaces you own, so your domain code never directly depends on external APIs.
- **Depend on abstractions**: High-level modules should depend on interfaces, not concrete implementations of external services.
- **Write learning tests**: Explore third-party library behavior with small tests to understand edge cases and document expected behavior.
- **Minimize boundary surface area**: Limit direct interaction with external APIs to a few adapter classes.
- **Translate external errors**: Catch exceptions from external libraries and convert them to domain-specific exceptions.
- **Design for change**: Assume external code will change and build boundaries that accommodate that change.

##### C#/.NET Examples

Example 1: Encapsulating a third-party library

**Poor design**

```csharp
public class NotificationService
{
    public void NotifyUserOfOrder(string email, Order order)
    {
        // Direct dependency on external library throughout the codebase
        ExternalEmailLib.Message msg = new ExternalEmailLib.Message(
            email,
            $"Order {order.Id} Confirmed",
            $"Thank you for your order of {order.ItemCount} items."
        );
        ExternalEmailLib.Client.Send(msg);
    }
}

public class PasswordResetService
{
    public void SendResetLink(string email, string token)
    {
        // Same external library used directly in another class
        ExternalEmailLib.Message msg = new ExternalEmailLib.Message(
            email,
            "Password Reset",
            $"Click here to reset: https://example.com/reset?token={token}"
        );
        ExternalEmailLib.Client.Send(msg);
    }
}
```

**Comment:**
External library usage is scattered across multiple classes. If the library changes its API or you need to switch providers, you must modify every class that uses it.

**Clean design**

```csharp
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body);
}

public class SmtpEmailSender : IEmailSender
{
    public Task SendAsync(string to, string subject, string body)
    {
        // External library usage encapsulated in one place
        var msg = new ExternalEmailLib.Message(to, subject, body);
        return ExternalEmailLib.Client.SendAsync(msg);
    }
}

public class NotificationService
{
    private readonly IEmailSender _emailSender;

    public NotificationService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task NotifyUserOfOrderAsync(string email, Order order)
    {
        string subject = $"Order {order.Id} Confirmed";
        string body = $"Thank you for your order of {order.ItemCount} items.";
        return _emailSender.SendAsync(email, subject, body);
    }
}
```

**Comment:**
The external library is wrapped in `IEmailSender`. Business logic depends only on the interface. Switching email providers requires only a new `IEmailSender` implementation.

---

Example 2: Translating external exceptions

**Poor design**

```csharp
public class PaymentService
{
    private readonly StripeClient _stripeClient;

    public async Task<PaymentResult> ProcessPaymentAsync(Order order)
    {
        // External exceptions leak into calling code
        var charge = await _stripeClient.ChargeAsync(
            order.CustomerId,
            order.Total
        );

        return new PaymentResult(charge.Id);
    }
}

// Caller must handle Stripe-specific exceptions
try
{
    await _paymentService.ProcessPaymentAsync(order);
}
catch (StripeCardDeclinedException ex) { /* Handle */ }
catch (StripeRateLimitException ex) { /* Handle */ }
catch (StripeApiException ex) { /* Handle */ }
```

**Comment:**
External exceptions propagate through the codebase. Callers must know about Stripe-specific exception types, coupling them to the external library.

**Clean design**

```csharp
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessAsync(string customerId, decimal amount);
}

public class StripePaymentGateway : IPaymentGateway
{
    private readonly StripeClient _stripeClient;

    public StripePaymentGateway(StripeClient stripeClient)
    {
        _stripeClient = stripeClient;
    }

    public async Task<PaymentResult> ProcessAsync(string customerId, decimal amount)
    {
        try
        {
            var charge = await _stripeClient.ChargeAsync(customerId, amount);
            return PaymentResult.Success(charge.Id);
        }
        catch (StripeCardDeclinedException ex)
        {
            throw new PaymentDeclinedException("Card was declined", ex);
        }
        catch (StripeRateLimitException ex)
        {
            throw new PaymentServiceUnavailableException("Payment service busy", ex);
        }
        catch (StripeApiException ex)
        {
            throw new PaymentFailedException($"Payment failed: {ex.Message}", ex);
        }
    }
}

// Domain exceptions
public class PaymentDeclinedException : Exception
{
    public PaymentDeclinedException(string message, Exception inner)
        : base(message, inner) { }
}
```

**Comment:**
External exceptions are caught at the boundary and translated to domain-specific exceptions. Callers deal with `PaymentDeclinedException`, not Stripe types.

---

Example 3: Minimizing external dependency footprint

**Poor design**

```csharp
// JSON library used directly throughout the codebase
public class ConfigService
{
    public string GetConfigJson() =>
        JsonConvert.SerializeObject(_config);
}

public class ApiController
{
    public string GetUserJson(User user) =>
        JsonConvert.SerializeObject(user);
}

public class CacheService
{
    public T GetOrCreate<T>(string key, Func<T> factory)
    {
        var cached = _cache.Get(key);
        if (cached != null)
            return JsonConvert.DeserializeObject<T>(cached);

        var value = factory();
        _cache.Set(key, JsonConvert.SerializeObject(value));
        return value;
    }
}
```

**Comment:**
`JsonConvert` (Newtonsoft.Json) is scattered everywhere. Switching to `System.Text.Json` requires changes across the entire codebase.

**Clean design**

```csharp
public interface IJsonSerializer
{
    string Serialize<T>(T value);
    T? Deserialize<T>(string json);
}

public class NewtonsoftJsonSerializer : IJsonSerializer
{
    public string Serialize<T>(T value) =>
        JsonConvert.SerializeObject(value);

    public T? Deserialize<T>(string json) =>
        JsonConvert.DeserializeObject<T>(json);
}

public class SystemTextJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, _options);

    public T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _options);
}

public class CacheService
{
    private readonly ICache _cache;
    private readonly IJsonSerializer _json;

    public CacheService(ICache cache, IJsonSerializer json)
    {
        _cache = cache;
        _json = json;
    }

    public T GetOrCreate<T>(string key, Func<T> factory)
    {
        var cached = _cache.Get(key);
        if (cached != null)
            return _json.Deserialize<T>(cached)!;

        var value = factory();
        _cache.Set(key, _json.Serialize(value));
        return value;
    }
}
```

**Comment:**
JSON serialization is abstracted behind `IJsonSerializer`. Switching from Newtonsoft to System.Text.Json requires only a new implementation and DI registration change.

---

Example 4: Writing learning tests for third-party code

**Poor design**

```csharp
// No tests - just assume the library works as expected
public class DateService
{
    public DateTime ParseUserInput(string input)
    {
        // How does NodaTime handle invalid input? Empty strings? Null?
        // We don't know until production fails
        return NodaTime.Parser.Parse(input).ToDateTimeUtc();
    }
}
```

**Comment:**
Assumptions about library behavior are untested. Edge cases and error handling are discovered in production.

**Clean design**

```csharp
// Learning tests - explore and document library behavior
public class NodaTimeLearningTests
{
    [Fact]
    public void Parse_ShouldThrowFormatException_WhenInputIsInvalid()
    {
        Assert.Throws<FormatException>(() =>
            NodaTime.Parser.Parse("not-a-date"));
    }

    [Fact]
    public void Parse_ShouldThrowArgumentNullException_WhenInputIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            NodaTime.Parser.Parse(null!));
    }

    [Fact]
    public void Parse_ShouldHandleIso8601Format()
    {
        var result = NodaTime.Parser.Parse("2024-01-15T10:30:00Z");

        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
            result.ToDateTimeUtc());
    }
}

// Now we can implement with confidence
public class DateService
{
    public DateTime? TryParseUserInput(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        try
        {
            return NodaTime.Parser.Parse(input).ToDateTimeUtc();
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
```

**Comment:**
Learning tests document expected library behavior. When the library updates, run these tests to detect breaking changes before they reach production.

---

Example 5: Testing with fakes at boundaries

**Poor design**

```csharp
[Fact]
public async Task ProcessOrder_ShouldSendConfirmationEmail()
{
    var service = new OrderService();

    // This actually sends an email during unit tests!
    await service.ProcessOrderAsync(testOrder);

    // No way to verify the email was sent correctly
}
```

**Comment:**
Tests depend on real external services, making them slow, unreliable, and potentially sending real emails.

**Clean design**

```csharp
public class FakeEmailSender : IEmailSender
{
    public List<(string To, string Subject, string Body)> SentEmails { get; } = new();

    public Task SendAsync(string to, string subject, string body)
    {
        SentEmails.Add((to, subject, body));
        return Task.CompletedTask;
    }
}

[Fact]
public async Task ProcessOrder_ShouldSendConfirmationEmail_WithOrderDetails()
{
    // Arrange
    var fakeEmailSender = new FakeEmailSender();
    var service = new OrderService(fakeEmailSender);
    var order = new Order { Id = 123, ItemCount = 2 };

    // Act
    await service.ProcessOrderAsync(order, "user@example.com");

    // Assert
    Assert.Single(fakeEmailSender.SentEmails);
    var (to, subject, body) = fakeEmailSender.SentEmails[0];
    Assert.Equal("user@example.com", to);
    Assert.Contains("Order 123", subject);
    Assert.Contains("2 items", body);
}
```

**Comment:**
Fake implementations allow testing business logic without external dependencies. Tests are fast, deterministic, and can verify exact behavior.

## Validation and Verification

### Automated Checks
- [ ] No direct references to third-party libraries in domain/business logic layers
- [ ] All external dependencies are injected via interfaces
- [ ] External exceptions are not caught outside of boundary/adapter classes
- [ ] Boundary classes are in dedicated namespace (e.g., `Infrastructure`, `Adapters`)

### Manual Review
- [ ] Each third-party library is wrapped in an adapter
- [ ] Interfaces represent domain concepts, not external API shapes
- [ ] Exception translation provides meaningful domain context
- [ ] Learning tests exist for critical third-party dependencies
- [ ] Fakes are available for all external boundaries

### Testing with Copilot
- [ ] Copilot suggests interface-based designs for external dependencies
- [ ] Copilot wraps third-party calls in adapter classes
- [ ] Copilot creates domain-specific exceptions at boundaries
- [ ] Copilot generates fakes for testing boundary code

## Best Practices to Remember

- Wrap third-party libraries in interfaces you control
- Translate external exceptions to domain-specific exceptions at the boundary
- Write learning tests to understand and document third-party behavior
- Use fakes in unit tests to isolate from external dependencies
- Keep boundary classes small and focused on translation, not business logic
- Assume external code will change and design boundaries to accommodate that change
