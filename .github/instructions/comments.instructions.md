---
description: 'Guidelines for good and bad comments in C#  following Clean Code principles'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **DO NOT** write comments to explain bad code; refactor it.
> - **REMOVE** commented-out code immediately.
> - **USE** comments only for legal headers, TODOs, or public API documentation.
> - **PREFER** pattern matching over complex boolean logic.

# Comments

Comments are often misunderstood. While they can be helpful, they should never compensate for poorly written code. This chapter explains when comments are appropriate, when they are harmful, and why self-explanatory code is always better than relying on comments.

##### Key Principles

- **Comments do not make up for bad code**: If code needs a comment to be understood, refactor the code instead.
- **Explain yourself in code**: Use meaningful names and clear logic rather than comments.
- **Good comments**: Legal notices, warnings, TODOs, and clarifications that cannot be expressed in code.
- **Bad comments**: Redundant, misleading, or outdated comments that clutter the code.
- **Avoid commented-out code**: It creates confusion and should be removed.
- **Prefer functions or variables over comments**: If a comment explains a block of code, consider extracting it into a well-named function.

##### C#/.NET Examples

Example 1: Replacing comments with clear code

Poor design

```csharp
// Check if user is active and has admin rights
if (user.Status == 1 && user.Role == 99)
{
    GrantAccess();
}
```

Clean design

```csharp
// Use pattern matching for clarity
if (user is { IsActive: true, IsAdmin: true })
{
    GrantAccess();
}
```

**Comment:**  
The code now explains itself through meaningful property names and pattern matching.

Example 2: Good comment usage

```csharp
// TODO: Replace hardcoded value with configuration setting
const int MaxRetryAttempts = 3;
```

**Comment:**  
This comment adds value because it indicates a future improvement that cannot be expressed in code.

Example 3: Avoid commented-out code

 Poor practice
 
 ```csharp
 // db.Delete(user); // Temporarily disabled
 ```

**Comment:**
Commented-out code should be removed. Use version control instead.

---

Example 4: XML documentation for public APIs

**Good practice**

```csharp
/// <summary>
/// Calculates the total price including tax and shipping.
/// </summary>
/// <param name="basePrice">The base price before tax and shipping.</param>
/// <param name="taxRate">The tax rate as a decimal (e.g., 0.08 for 8%).</param>
/// <param name="shippingCost">The shipping cost in dollars.</param>
/// <returns>The total price including tax and shipping.</returns>
/// <exception cref="ArgumentOutOfRangeException">Thrown when basePrice or shippingCost is negative.</exception>
public decimal CalculateTotalPrice(decimal basePrice, decimal taxRate, decimal shippingCost)
{
    ArgumentOutOfRangeException.ThrowIfNegative(basePrice);
    ArgumentOutOfRangeException.ThrowIfNegative(shippingCost);

    return basePrice * (1 + taxRate) + shippingCost;
}
```

**Comment:**
XML documentation is appropriate for public APIs that others will consume. Keep it concise and factual.

---

Example 5: Avoiding redundant comments

**Poor practice**

```csharp
// Get the user by ID
var user = GetUserById(id);

// Check if user is null
if (user == null)
{
    // Return not found
    return NotFound();
}
```

**Clean design**

```csharp
var user = GetUserById(id);

if (user == null){
    return NotFound();
}
   
```

**Comment:**
Comments that merely restate what the code obviously does add no value and create maintenance burden.

## Validation and Verification

### Automated Checks
- [ ] No commented-out code in the codebase
- [ ] XML documentation present for all public APIs
- [ ] No TODO comments older than 30 days
- [ ] Comments do not exceed code by line count

### Manual Review
- [ ] All comments add genuine value, not restate code
- [ ] Complex algorithms have explanatory comments
- [ ] Legal/copyright headers are present where required
- [ ] No misleading or outdated comments

## Best Practices to Remember

- Strive for self-documenting code.
- Use comments only when necessary and valuable.
- Remove outdated or irrelevant comments.
- Never leave commented-out code in production.
- Prefer refactoring over explaining complex logic with comments.