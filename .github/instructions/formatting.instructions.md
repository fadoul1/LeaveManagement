---
description: 'Guidelines for code formatting in C# following Clean Code principles'
applyTo: '**/*.cs'
---

## System Directives

> **Copilot Instruction:**
> - **ORGANIZE** code vertically by relevance.
> - **KEEP** lines short.
> - **USE** consistent indentation.
> - **USE** file-scoped namespaces to reduce nesting.
> - **USE** named arguments for long parameter lists.

# Formatting

Formatting is not just about aesthetics—it’s about communication. Proper formatting makes code easier to read, understand, and maintain. This chapter explains vertical and horizontal formatting principles and how consistent style improves team collaboration.

##### Key Principles

- **Formatting communicates intent**: Well-structured code guides the reader through logic and relationships.

- **Vertical formatting**: Organize code top-to-bottom like a newspaper—important details first, then supporting details.

- **Horizontal formatting**: Keep lines short and avoid excessive horizontal scrolling.

- **Consistent indentation and spacing**: Improves readability and reduces cognitive load.

- **Group related concepts together**: Functions and variables that work together should be close to each other.

- **Follow team conventions**: Agree on formatting rules and enforce them consistently.
  

## C#/.NET Examples

Example 1: Vertical formatting

**Poor design**

```csharp
public class OrderProcessor { public void Process(Order o){Validate(o);Save(o);Notify(o);}private void Validate(Order o){/*...*/}private void Save(Order o){/*...*/}private void Notify(Order o){/*...*/}}
```

**Clean design**

```csharp
namespace MyApp.Orders; // File-scoped namespace reduces indentation

public class OrderProcessor
{
    public void Process(Order order)
    {
        Validate(order);
        Save(order);
        Notify(order);
    }

    private void Validate(Order order) {}

    private void Save(Order order) {}

    private void Notify(Order order) {}
}
```

**Comment:**
Proper line breaks and indentation make the code readable and maintainable.

Example 2: Horizontal formatting

**Poor design**

```csharp
var result=Calculate(order.Amount,order.TaxRate,order.Discount,order.ShippingCost,order.Currency);
```

**Clean design**

```csharp
var result = Calculate(
    amount: order.Amount,
    taxRate: order.TaxRate,
    discount: order.Discount,
    shippingCost: order.ShippingCost,
    currency: order.Currency);
```

**Comment:**
Breaking long argument lists into multiple lines and using named arguments improves clarity.

Example 3: Consistent spacing and alignment

**Poor design**

```csharp
if(order.Status==OrderStatus.Pending){Process(order);}
```

**Clean design**

```csharp
if (order.Status == OrderStatus.Pending)
{
    Process(order);
}
```

**Comment:**
Consistent spacing around operators and braces improves readability.

## Validation and Verification

### Automated Checks
- [ ] File-scoped namespaces used consistently
- [ ] Named arguments used for methods with 3+ parameters
- [ ] Lines do not exceed 120 characters
- [ ] Consistent indentation (4 spaces for C#)
- [ ] Opening braces on new line (Allman style)

### Manual Review
- [ ] Related code blocks are grouped together
- [ ] Vertical whitespace used to separate logical sections
- [ ] Consistent formatting throughout the codebase
- [ ] Team formatting conventions are followed

## Best Practices to Remember

- Use vertical space to separate concepts and horizontal space for clarity.
- Keep lines short and avoid deep nesting.
- Follow a consistent style guide (e.g., Microsoft C# Coding Conventions).
- Use automated tools (linters, formatters) to enforce formatting rules.
