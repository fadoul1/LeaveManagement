---
description: 'Guidelines for writing functions in C#  following Clean Code principles'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **Functions MUST** do one thing only.
> - **Functions SHOULD NOT** have more than 3 arguments.
> - **Functions MUST NOT** produce unexpected side effects.
> - **PREFER** `record` types for data transfer objects (DTOs).
> - **USE** expression-bodied members `=>` for simple one-line functions.

# Functions

Functions are the building blocks of clean code. This chapter emphasizes writing small, focused functions that do one thing and do it well. It covers principles such as minimizing arguments, avoiding side effects, and maintaining a single level of abstraction within each function.

##### Key Principles

- **Functions should be small**: Ideally, just a few lines of code.
- **Do one thing**: A function should have a single responsibility.
- **Use descriptive names**: Names should clearly express the function’s purpose.
- **Limit the number of arguments**: Prefer zero or one argument; avoid more than three.
- **Avoid side effects**: Functions should not change state unexpectedly.
- **Command-Query Separation**: Functions should either perform an action or return data, not both.
- **Prefer exceptions over error codes**: Makes error handling cleaner and more expressive.
- **Don’t repeat yourself (DRY)**: Eliminate duplication through refactoring.

##### C#/.NET Examples

Example 1: Small and focused function

 Poor design

 ```csharp
 public void ProcessOrder(Order order)
 {
    // Validation logic
    // Save order
    // SendConfirmationEmail
 }
 ```

 **Comment:**  
 This function does multiple things: validation, persistence, and email sending.

 Clean design

 ```csharp
 public void ProcessOrder(Order order)
 {
    ValidateOrder(order);
    SaveOrder(order);
    NotifyCustomer(order);
 }

 private void ValidateOrder(Order order) { /* ... */ }
 private void SaveOrder(Order order) { /* ... */ }
 private void NotifyCustomer(Order order) { /* ... */ }
 ```

 **Comment:**  
 Each function now has a single responsibility, improving readability and maintainability.

Example 2: Limiting arguments

 Poor design

   ```csharp
    public void CreateUser(string firstName, string lastName, int age, string email, string phone)
    {
        // ...
    }
  ```

 Clean design

   ```csharp
   // Use record for DTOs
   public record User(string FirstName, string LastName, int Age, string Email, string Phone);

   public void CreateUser(User user)
   {
        // ...
   }
   ```

 **Comment:**  
 Encapsulate related data in an object (preferably a `record`) to reduce argument count and improve clarity.

Example 3: Avoiding side effects

 Poor design

 ```csharp
 public bool IsValidUser(User user)
 {
    if (user.Age < 18)
    {
        user.IsActive = false; // Side effect!
        return false;
    }
    return true;
 }
 ```

 Clean design

 ```csharp
 // Expression-bodied member for simple logic
 public bool IsValidUser(User user) => user.Age >= 18;
 ```

 **Comment:**
 Validation should not modify the object's state.

---

Example 4: Async/await best practices

**Poor design**

```csharp
public async Task<User> GetUserAsync(int id)
{
    var user = await _repository.GetUserAsync(id);
    return user; // Unnecessary await
}

public void ProcessOrder(Order order)
{
    SaveOrderAsync(order).Wait(); // Blocking async call
}
```

**Clean design**

```csharp
// Avoid unnecessary async when just returning the Task
public Task<User> GetUserAsync(int id)
    => _repository.GetUserAsync(id);

// Make the calling method async too
public async Task ProcessOrderAsync(Order order)
{
    await SaveOrderAsync(order);
}
```

**Comment:**
Avoid unnecessary async/await when just returning a Task. Never use `.Wait()` or `.Result` as they block threads.

---

Example 5: Exception handling over error codes

**Poor design**

```csharp
public int SaveUser(User user)
{
    if (user == null) return -1;
    if (!IsValid(user)) return -2;

    try
    {
        _repository.Save(user);
        return 0; // Success
    }
    catch
    {
        return -3;
    }
}
```

**Clean design**

```csharp
public void SaveUser(User user)
{
    ArgumentNullException.ThrowIfNull(user);

    if (!IsValid(user))
        throw new ValidationException("User data is invalid");

    _repository.Save(user);
}
```

**Comment:**
Exceptions provide better error context and stack traces. Error codes force callers to check return values.

---

Example 6: LINQ usage

**Poor design**

```csharp
public List<User> GetActiveAdminUsers(List<User> users)
{
    var result = new List<User>();
    foreach (var user in users)
    {
        if (user.IsActive && user.IsAdmin)
        {
            result.Add(user);
        }
    }
    return result;
}
```

**Clean design**

```csharp
public IEnumerable<User> GetActiveAdminUsers(IEnumerable<User> users)
    => users.Where(user => user.IsActive && user.IsAdmin);
```

**Comment:**
LINQ provides a more expressive, functional approach. Return `IEnumerable<T>` for better flexibility.

## Validation and Verification

### Automated Checks
- [ ] All functions have a single, clear responsibility
- [ ] No function has more than 3 parameters
- [ ] All functions use expression-bodied syntax when appropriate
- [ ] Async methods return Task or Task<T>
- [ ] No `.Wait()` or `.Result()` calls on Tasks

### Manual Review
- [ ] Function names clearly describe what they do
- [ ] Functions produce no unexpected side effects
- [ ] Command-Query Separation is maintained
- [ ] Error handling uses exceptions, not error codes
- [ ] No code duplication between functions

## Best Practices to Remember

- Keep functions short and focused.
- Avoid unnecessary complexity and nesting.
- Use meaningful names that describe intent.
- Refactor aggressively to maintain clarity.
- Test functions independently.