---
description: 'Guidelines explains how good design emerges from the consistent application of simple principles'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **REFACTOR** continuously to remove duplication.
> - **ENSURE** code expresses intent clearly.
> - **USE** switch expressions for cleaner conditional logic.
> - **USE** pattern matching to simplify complex conditions.

# Emergence

This chapter explains how good design emerges from the consistent application of simple principles. Rather than relying on upfront complexity, clean architecture evolves through continuous refactoring and adherence to fundamental rules. The goal is to keep the system simple, expressive, and free of duplication.

###### Key Principles

- **Emergent design**: A clean design is not imposed; it evolves through disciplined practices.
- **Four rules of simple design**:
    - **Runs all the tests**
    - **Contains no duplication**
    - **Expresses intent clearly**
    - **Minimizes the number of classes and methods**

- **Refactoring is essential**: Regularly improve the structure without changing behavior.
- **Expressiveness matters**: Code should communicate its purpose clearly to humans, not just machines. 

###### C#/.NET Examples

Example 1: Removing duplication

**Before (duplicated logic)**
  
```csharp
public decimal CalculateDiscount(Customer customer)
{
    if (customer.IsPremium)
        return customer.PurchaseAmount * 0.10m;

    return customer.PurchaseAmount * 0.05m;
}

public decimal CalculateLoyaltyBonus(Customer customer)
{
    if (customer.IsPremium)
        return customer.PurchaseAmount * 0.02m;

    return customer.PurchaseAmount * 0.01m;
}
```

**After (refactored)**

```csharp
public decimal CalculateRate(
	Customer customer, 
	decimal premiumRate, 
	decimal standardRate)
{
    // Use switch expression for clarity
    return customer switch
    {
        { IsPremium: true } => customer.PurchaseAmount * premiumRate,
        _ => customer.PurchaseAmount * standardRate
    };
}

public decimal CalculateDiscount(Customer customer) 
	=> CalculateRate(customer, premiumRate: 0.10m, standardRate: 0.05m);
```

**Comment :**  
Duplication is eliminated by extracting common logic into a reusable method.

Example 2: Expressive code

**Poor design**

```csharp
if (x > 100 && y == 1)
{
    // ...
}
```

**Clean design**

```csharp
if (IsHighValueOrder(order) && IsFirstPurchase(customer))
{
    // Apply special discount
}
```

**Comment:**
Expressive method names or pattern matching make the intent clear without comments.

---

## Validation and Verification

### Automated Checks
- [ ] No duplicated code blocks (DRY principle)
- [ ] Switch expressions used instead of switch statements where appropriate
- [ ] Pattern matching used for type checks
- [ ] All tests pass

### Manual Review
- [ ] Code expresses intent clearly without comments
- [ ] Classes and methods are minimal and focused
- [ ] Design has evolved through refactoring, not upfront complexity
- [ ] No premature abstractions

## Best Practices to Remember

- Continuously refactor to maintain simplicity.
- Eliminate duplication aggressively.
- Write expressive code that communicates intent.
- Keep classes and methods minimal and focused.
- Let design emerge through disciplined practices, not upfront complexity.