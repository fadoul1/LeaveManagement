---
description: 'Guidelines for designing clean, cohesive classes following SOLID principles in C#'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **CLASSES MUST** follow Single Responsibility Principle (SRP).
> - **PREFER** composition over inheritance.
> - **USE** `record` types for immutable data objects.
> - **KEEP** classes small and cohesive.
> - **MAKE** fields private; expose through properties.
> - **AVOID** classes with many dependencies (max 3-4).

# Classes

Classes are the fundamental building blocks of object-oriented design. This chapter explores how to organize classes to maximize cohesion, minimize coupling, and create systems that are easy to understand and change.

##### Key Principles

- **Single Responsibility Principle (SRP)**: A class should have only one reason to change.
- **High cohesion**: Class members should be closely related and work together.
- **Low coupling**: Classes should depend on abstractions, not concrete implementations.
- **Encapsulation**: Hide implementation details behind well-defined interfaces.
- **Small classes**: Classes should be small, focused, and do one thing well.
- **Open/Closed Principle**: Open for extension, closed for modification.
- **Dependency Inversion**: Depend on abstractions, not concretions.

##### C#/.NET Examples

Example 1: Single Responsibility Principle

**Poor design**

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    // Validation logic
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) &&
               !string.IsNullOrEmpty(Email) &&
               Email.Contains("@");
    }

    // Database operations
    public void Save()
    {
        using var connection = new SqlConnection("...");
        // SQL code...
    }

    // Email logic
    public void SendWelcomeEmail()
    {
        var smtp = new SmtpClient();
        // Email sending code...
    }

    // Formatting logic
    public string ToHtml()
    {
        return $"<div>{Name} - {Email}</div>";
    }
}
```

**Clean design**

```csharp
// Single responsibility: represent user data
public record User(int Id, string Name, string Email);

// Single responsibility: validate users
public class UserValidator
{
    public bool IsValid(User user)
    {
        return !string.IsNullOrEmpty(user.Name) &&
               !string.IsNullOrEmpty(user.Email) &&
               user.Email.Contains("@");
    }
}

// Single responsibility: persist users
public class UserRepository
{
    public void Save(User user)
    {
        // Database operations
    }
}

// Single responsibility: send user-related emails
public class UserEmailService
{
    public void SendWelcomeEmail(User user)
    {
        // Email sending logic
    }
}

// Single responsibility: format users for display
public class UserHtmlFormatter
{
    public string ToHtml(User user)
        => $"<div>{user.Name} - {user.Email}</div>";
}
```

**Comment:**
Each class has one reason to change. Changes to validation don't affect persistence, email, or formatting.

---

Example 2: High cohesion

**Poor design**

```csharp
public class OrderProcessor
{
    private readonly IEmailService _emailService;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IInventoryService _inventoryService;
    private readonly IUserService _userService;
    private readonly IReportGenerator _reportGenerator;
    private readonly ILogger _logger;

    // Some methods use email, some use payment, some use inventory...
    // Low cohesion - not all fields are used together
}
```

**Clean design**

```csharp
// High cohesion - all fields used together for order processing
public class OrderProcessor
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderValidator _orderValidator;
    private readonly IOrderNotifier _orderNotifier;

    public async Task ProcessAsync(Order order)
    {
        await _orderValidator.ValidateAsync(order);
        await _orderRepository.SaveAsync(order);
        await _orderNotifier.NotifyAsync(order);
    }
}

// Payment handling separated into its own cohesive class
public class PaymentProcessor
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPaymentRepository _paymentRepository;

    public async Task ProcessPaymentAsync(Order order)
    {
        // All fields used together
    }
}
```

**Comment:**
High cohesion means all class members work together toward a single purpose. If fields aren't used together, split the class.

---

Example 3: Composition over inheritance

**Poor design**

```csharp
public class Animal
{
    public virtual void Move() { }
    public virtual void MakeSound() { }
}

public class Bird : Animal
{
    public override void Move() { /* Fly */ }
    public override void MakeSound() { /* Chirp */ }
}

public class Fish : Animal
{
    public override void Move() { /* Swim */ }
    public override void MakeSound() { /* Nothing - fish don't make sounds */ }
}

// What about a penguin? It's a bird but doesn't fly...
public class Penguin : Bird
{
    public override void Move() { /* Swim? Walk? */ }
}
```

**Clean design**

```csharp
// Composition with interfaces
public interface IMovable
{
    void Move();
}

public interface ISoundMaker
{
    void MakeSound();
}

public class FlyingBehavior : IMovable
{
    public void Move() => Console.WriteLine("Flying");
}

public class SwimmingBehavior : IMovable
{
    public void Move() => Console.WriteLine("Swimming");
}

public class Bird
{
    private readonly IMovable _movement;
    private readonly ISoundMaker _soundMaker;

    public Bird(IMovable movement, ISoundMaker soundMaker)
    {
        _movement = movement;
        _soundMaker = soundMaker;
    }

    public void Move() => _movement.Move();
    public void MakeSound() => _soundMaker.MakeSound();
}

// Easy to compose different behaviors
var eagle = new Bird(new FlyingBehavior(), new ChirpSound());
var penguin = new Bird(new SwimmingBehavior(), new ChirpSound());
```

**Comment:**
Composition is more flexible than inheritance. You can change behavior at runtime and avoid deep inheritance hierarchies.

---

Example 4: Encapsulation with properties

**Poor design**

```csharp
public class BankAccount
{
    public decimal Balance; // Public field - no encapsulation!

    public void Withdraw(decimal amount)
    {
        Balance -= amount; // No validation
    }
}

// Caller can bypass business logic
account.Balance = -1000; // Negative balance!
```

**Clean design**

```csharp
public class BankAccount
{
    private decimal _balance;

    public decimal Balance => _balance; // Read-only property

    public void Deposit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        _balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (amount > _balance)
            throw new InsufficientFundsException(_balance, amount);

        _balance -= amount;
    }
}
```

**Comment:**
Encapsulation protects invariants. Balance can only change through validated methods, not direct assignment.

---

Example 5: Dependency Inversion Principle

**Poor design**

```csharp
// High-level module depends on low-level module
public class OrderService
{
    private readonly SqlOrderRepository _repository; // Concrete dependency

    public OrderService()
    {
        _repository = new SqlOrderRepository(); // Tightly coupled
    }

    public void ProcessOrder(Order order)
    {
        _repository.Save(order);
    }
}
```

**Clean design**

```csharp
// Both depend on abstraction
public interface IOrderRepository
{
    void Save(Order order);
    Order GetById(int id);
}

public class SqlOrderRepository : IOrderRepository
{
    public void Save(Order order) { /* SQL implementation */ }
    public Order GetById(int id) { /* SQL implementation */ }
}

public class OrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public void ProcessOrder(Order order)
    {
        _repository.Save(order);
    }
}

// Easy to swap implementations or test
var service = new OrderService(new SqlOrderRepository());
var testService = new OrderService(new InMemoryOrderRepository());
```

**Comment:**
Depend on abstractions (interfaces), not concrete classes. This enables testing and makes the system flexible.

---

Example 6: Using records for immutable data

**Poor design**

```csharp
public class Money
{
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; set; } // Mutable!
    public string Currency { get; set; } // Mutable!

    // Have to implement Equals, GetHashCode manually
    public override bool Equals(object obj) { /* ... */ }
    public override int GetHashCode() { /* ... */ }
}
```

**Clean design**

```csharp
// Record provides immutability and value equality automatically
public record Money(decimal Amount, string Currency)
{
    // Can add validation in constructor
    public Money(decimal Amount, string Currency) : this()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Amount);
        ArgumentException.ThrowIfNullOrEmpty(Currency);

        this.Amount = Amount;
        this.Currency = Currency;
    }

    // Can add computed properties
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");

        return this with { Amount = Amount + other.Amount };
    }
}
```

**Comment:**
Records are perfect for immutable data objects. They provide value equality, immutability, and concise syntax.

## Validation and Verification

### Automated Checks
- [ ] Classes have fewer than 200 lines of code
- [ ] Classes have fewer than 10 methods
- [ ] Constructors have fewer than 4 parameters
- [ ] All fields are private
- [ ] Record types used for immutable data objects

### Manual Review
- [ ] Each class has a single, clear responsibility
- [ ] Class members are highly cohesive
- [ ] Classes depend on abstractions (interfaces)
- [ ] No God classes (classes that do everything)
- [ ] Composition preferred over inheritance

## Best Practices to Remember

- Keep classes small and focused on a single responsibility
- Maximize cohesion—all class members should work together
- Minimize coupling—depend on abstractions, not concrete classes
- Protect invariants through encapsulation
- Prefer composition over inheritance for flexibility
- Use records for immutable value objects
- Apply SOLID principles to create maintainable designs
