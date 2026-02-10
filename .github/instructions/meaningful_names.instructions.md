---
description: 'Guidelines for C# meaningful naming conventions following Clean Code principles'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **ALWAYS** use names that reveal intent.
> - **DO NOT** use single-letter variable names (except in loops).
> - **AVOID** encodings like Hungarian notation.
> - **PREFER** `DateOnly` or `TimeOnly` over strings for date/time values.
> - **USE** `Is`, `Has`, `Can`, `Should` prefixes for boolean properties.
> - **ADD** `Async` suffix to asynchronous method names.

# Meaningful Names

This chapter focuses on the art and discipline of naming in software. Good names make code easier to read, understand, and maintain. The chapter provides practical rules and examples for choosing names that reveal intent, avoid confusion, and communicate clearly to other developers.

##### Key Principles

- **Names should reveal intent**: A good name answers the questions “Why does it exist?”, “What does it do?”, and “How is it used?”
- **Avoid disinformation**: Do not use names that are misleading or have established meanings in other contexts.
- **Make meaningful distinctions**: Avoid names that differ only by a number, letter, or trivial change.
- **Use pronounceable and searchable names**: If you can’t say it, you can’t discuss it. If you can’t search for it, you can’t find it.
- **Avoid encodings and mental mapping**: Don’t encode type or scope in names, and don’t force readers to translate names in their heads.
- **Be consistent**: Use the same word for the same concept throughout your codebase.
- **Add meaningful context**: When a name alone isn’t enough, provide context through class, namespace, or prefix.
- **Don’t add gratuitous context**: Avoid unnecessary prefixes or suffixes that add no value.

##### C#/.NET Examples

Example 1: Revealing intent

**Poor naming**

```csharp
 int d; // elapsed time in days
 var genymdhms = "2023-11-07T15:30:00";
 public class DataManager { }
 public class DataProcessor { }
 ```

**Good naming**

```csharp
 int elapsedTimeInDays;
 var generationTimestamp = "2023-11-07T15:30:00";
 public class UserDataManager { }
 public class UserDataProcessor { }
 ```
 **Comment:**
 Adding context clarifies the purpose and scope of each class.

---

Example 2: Boolean naming

**Poor naming**

```csharp
public class User
{
    public bool Active { get; set; }
    public bool Admin { get; set; }
    public bool Delete { get; set; }
    public bool Flag { get; set; }
}
```

**Good naming**

```csharp
public class User
{
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }
    public bool CanDelete { get; set; }
    public bool HasVerifiedEmail { get; set; }
}
```

**Comment:**
Boolean properties should clearly indicate they return true/false using prefixes like `Is`, `Has`, `Can`, or `Should`.

---

Example 3: Async method naming

**Poor naming**

```csharp
public Task<User> GetUser(int id) { }
public Task Save(Order order) { }
public Task<List<Product>> LoadProducts() { }
```

**Good naming**

```csharp
public async Task<User> GetUserAsync(int id) { }
public async Task SaveAsync(Order order) { }
public async Task<List<Product>> LoadProductsAsync() { }
```

**Comment:**
Asynchronous methods should have the `Async` suffix to make it immediately clear they return a Task and should be awaited.

---

Example 4: Collection naming

**Poor naming**

```csharp
public class OrderManager
{
    public List<Order> Order { get; set; }
    public IEnumerable<string> Item { get; set; }
    public Dictionary<int, User> UserMap { get; set; }
}
```

**Good naming**

```csharp
public class OrderManager
{
    public List<Order> Orders { get; set; }
    public IEnumerable<string> Items { get; set; }
    public Dictionary<int, User> UsersById { get; set; }
}
```

**Comment:**
Collections should be plural. Dictionary names should indicate the key-value relationship when not obvious.

---

Example 5: Avoiding abbreviations

**Poor naming**

```csharp
var custAddr = new Address();
var svcMgr = new ServiceManager();
int numOfRecs = GetRecordCount();
```

**Good naming**

```csharp
var customerAddress = new Address();
var serviceManager = new ServiceManager();
int numberOfRecords = GetRecordCount();
```

**Comment:**
Avoid abbreviations unless they're universally understood (like `Id`, `Url`, `Html`). Full words improve readability.

## Validation and Verification

### Automated Checks
- [ ] All names are pronounceable English words or phrases
- [ ] No single-letter variables (except loop counters `i`, `j`, `k`)
- [ ] No Hungarian notation or type encoding
- [ ] Boolean properties use appropriate prefixes
- [ ] Async methods end with `Async` suffix

### Manual Review
- [ ] Names reveal intent without requiring comments
- [ ] Names are searchable and unique enough to find with Ctrl+F
- [ ] Consistent terminology used throughout the codebase
- [ ] No mental mapping required to understand names

## Best Practices to Remember

- Take time to choose names; don’t settle for the first idea.
- Refactor names when you find better ones.
- Prefer clarity over cleverness.
- Use domain language and standard terminology.
- Avoid abbreviations unless they are universally understood.