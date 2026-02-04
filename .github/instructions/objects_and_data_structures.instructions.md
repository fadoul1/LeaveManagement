---
description: "Guidelines on Objects vs Data Structures: how to hide implementation, apply Demeter, and avoid hybrid designs."
applyTo: "**/*.cs"
---

## System Directives
> **Copilot Instruction:**
> - **PREFER** object interfaces that **hide data** and expose **behaviors**.
> - **AVOID** trivial getters/setters that leak implementation details.
> - **FOLLOW** Law of Demeter: **talk to friends, not to strangers**.
> - **SEPARATE** domain behavior from **DTO/Active Record** persistence.
> - **CHOOSE** OO when you add **new types**; **choose** procedural when you add **new behaviors**.

## Objects and Data Structures — Core Idea
Objects hide internal representation and expose meaningful operations.
Data structures expose data and have minimal behavior.  
Choose the one that fits the **change you expect**:
- **OO** makes it easy to add **new classes/types** without changing existing functions.
- **Procedural** makes it easy to add **new functions/behaviors** without changing existing data structures.

---

## 1) Data Abstraction (not just getters/setters)
**Goal:** Hide representation; expose intent.

### Poor design (concrete exposure)
```csharp
public class Point
{
    public double X {get;set;}
    public double Y {get;set;}
}
```

### Clean design (abstract interface)
```csharp
public interface IPoint
{
    double GetX();
    double GetY();
    void SetCartesian(double x, double y);

    double GetR();
    double GetTheta();
    void SetPolar(double r, double theta);
}
```
**Comment:** The abstraction does not betray whether the implementation is rectangular or polar; it enforces atomic updates and intent-driven access.

#### Prefer abstract data views
**Concrete**:
```csharp
public interface IVehicle
{
    double GetFuelTankCapacityGallons();
    double GetGallonsInTank();
}
```
**Abstract**:
```csharp
public interface IVehicle
{
    double GetPercentFuelRemaining();
}
```
**Comment:** Expose **meaning**, not storage details.

---

## 2) Object/Data Anti-Symmetry
### Procedural style (easy to add functions, hard to add types)
```csharp
public record Square(Point TopLeft, double Side);
public record Rectangle(Point TopLeft, double Height, double Width);
public record Circle(Point Center, double Radius);

public static class Geometry
{
    private const double PI = Math.PI;

    public static double Area(object shape) => shape switch
    {
        Square s    => s.Side * s.Side,
        Rectangle r => r.Height * r.Width,
        Circle c    => PI * c.Radius * c.Radius,
        _           => throw new ArgumentOutOfRangeException(nameof(shape))
    };
}
```

### OO style (easy to add types, hard to add functions)
```csharp
public interface IShape { double Area(); }

public sealed class Square : IShape
{
    private readonly Point _topLeft;
    private readonly double _side;
    public Square(Point topLeft, double side) => (_topLeft, _side) = (topLeft, side);
    public double Area() => _side * _side;
}

public sealed class Rectangle : IShape
{
    private readonly Point _topLeft;
    private readonly double _height;
    private readonly double _width;
    public Rectangle(Point topLeft, double height, double width) => (_topLeft, _height, _width) = (topLeft, height, width);
    public double Area() => _height * _width;
}

public sealed class Circle : IShape
{
    private readonly Point _center;
    private readonly double _radius;
    public Circle(Point center, double radius) => (_center, _radius) = (center, radius);
    public double Area() => Math.PI * _radius * _radius;
}
```

**Heuristic:** Use OO when you anticipate *new shapes*; use procedural when you anticipate *new operations* (e.g., `Perimeter`, `BoundingBox`).

---

## 3) Law of Demeter (LoD)
**Rule:** A method should only talk to:
- Its own class,
- Objects it creates,
- Arguments passed in,
- Its own fields.  
Avoid reaching through objects returned from those calls (no long chains).

### Train-wreck (to avoid)
```csharp
var outputDir = context.GetOptions().GetScratchDir().GetAbsolutePath();
```

### Better (split or encapsulate)
```csharp
var opts = context.GetOptions();
var scratchDir = opts.GetScratchDir();
var outputDir = scratchDir.GetAbsolutePath();
```

### Best (tell, don’t ask): encapsulate intent
```csharp
// What you really wanted:
using var bos = context.CreateScratchFileStream(classFileName);
```
**Comment:** Encapsulate navigation and low-level details (paths, separators, streams) inside the owning object.

---

## 4) Avoid Hybrid Designs
Hybrids (half-object, half-data: significant methods plus public fields or trivial accessors that leak internals) suffer from both worlds: hard to add functions **and** hard to add types. **Avoid them.** Decide **explicitly**:  
- **DTO/Record** for data transfer/persistence shape,  
- **Domain Object** for behavior with hidden state.

---

## 5) DTOs & Active Record
### DTO (Data Transfer Object)
Simple shapes to carry data across boundaries (DB, APIs, messaging).
```csharp
public record Address(string Street, string StreetExtra, string City, string State, string Zip);
```

### Active Record
Persistence-oriented structure mirroring tables (fields + `Save`, `Find`).
- Keep business rules **out** of Active Record.
- Wrap AR instances in **domain objects/services** that enforce invariants and behavior.

---

## Validation and Verification

### Automated Checks
- [ ] No public fields on domain objects (records for DTOs are OK).
- [ ] No method chains across foreign objects (LoD).
- [ ] No trivial pass-through getters/setters that expose representation.
- [ ] OO vs Procedural choice justified by expected axis of change (new types vs new operations).
- [ ] Persistence (Active Record/DTO) separated from domain behavior.

### Manual Review
- [ ] Interface communicates **intent**, not storage.
- [ ] Navigation through object graphs is encapsulated behind **intent methods**.
- [ ] No hybrid classes: each type is **clearly** a DTO, AR, or domain object.
- [ ] Adding a new **type** or a new **operation** doesn’t trigger broad ripple changes (design aligned to change axis).
- [ ] Paths, streams, formatting are **localized** in their owning objects (no scattered low-level details).

---

## Best Practices to Remember
- Hide implementation; expose meaningful operations.
- Favor **records/DTOs** for data transfer; keep domain rules in **objects/services**.
- Apply **Law of Demeter**: tell objects *what* to do.
- Choose architecture by **anticipated change** (types vs operations).
- Resist hybrids; commit to either object or data structure.
- Encapsulate low-level details (file paths, IO) behind **intentful APIs**.
