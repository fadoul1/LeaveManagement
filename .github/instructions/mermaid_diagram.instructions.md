# Copilot Instructions — Mermaid Diagrams

## 🎯 Objective

Generate **valid, readable, and maintainable Mermaid diagrams** following consistent conventions suitable for technical documentation and architecture diagrams.

---

## ✅ General Rules

1. **Always specify the diagram type**
   Choose the appropriate diagram type based on the context:
   | Type | Use Case |
   |------|----------|
   | `flowchart TD` | Process flows, architecture layers, decision trees |
   | `sequenceDiagram` | API calls, service interactions, request/response flows |
   | `classDiagram` | Domain models, class relationships, inheritance |
   | `erDiagram` | Database schemas, entity relationships |
   | `stateDiagram-v2` | State machines, workflow states, lifecycle |
   | `gitGraph` | Branching strategies, release flows |
   | `pie` | Distribution, percentages, statistics |
   | `gantt` | Project timelines, task scheduling |

2. **Quote labels containing special characters**
   Wrap text in double quotes when it contains: `{ }`, `/`, `:`, `#`, `()`, or spaces.

   **Good:**
   ```mermaid
   A1["PUT /api/leaves/{id}/approve"]
   ```

3. **Prefer meaningful identifiers + labels**
   * Technical ID: `A1`, `B2`, `C3`, etc.
   * Human label: clear, concise, and descriptive.

---

## 📊 Diagram Type Guidelines

### Flowchart (Architecture & Processes)

Use for visualizing application layers, data flows, and decision logic.

```mermaid
flowchart TD
    subgraph API["API Layer"]
        A1["PUT /resource/{id}/action"]
    end

    subgraph Application["Application Layer"]
        B1[Command]
        B2[Handler]
        B3[Validator]
    end

    subgraph Domain["Domain Layer"]
        C1[Aggregate]
        C2[Repository Interface]
    end

    A1 --> B1 --> B2 --> B3
    B2 --> C2 --> C1
```

**Arrow conventions:**
| Arrow  | Meaning                                  |
| ------ | ---------------------------------------- |
| `-->`  | Normal control/data flow                 |
| `-.->` | Asynchronous / event-driven flow         |
| `==>`  | Strong dependency or commitment          |
| `-.-`  | Relationship / reference (not execution) |

---

### Sequence Diagram (Interactions)

Use for API calls, service-to-service communication, and request/response flows.

```mermaid
sequenceDiagram
    autonumber
    participant C as Client
    participant API as LeaveController
    participant H as ApproveLeaveHandler
    participant R as LeaveRepository
    participant DB as PostgreSQL

    C->>API: PUT /api/leaves/{id}/approve
    API->>H: Send(ApproveLeaveCommand)
    H->>R: GetByIdAsync(id)
    R->>DB: SELECT * FROM leaves
    DB-->>R: Leave entity
    R-->>H: Leave
    H->>H: Validate & Update status
    H->>R: UpdateAsync(leave)
    R->>DB: UPDATE leaves
    DB-->>R: Success
    R-->>H: Updated Leave
    H-->>API: LeaveResponse
    API-->>C: 200 OK
```

**Conventions:**
- Use `autonumber` for step tracking
- `->>` for synchronous calls, `-->>` for responses
- `--)` for async messages (fire-and-forget)
- Use `participant` aliases for readability
- Group related steps with `rect` or `alt`/`else`

---

### Class Diagram (Domain Models)

Use for entity relationships, inheritance, and domain modeling.

```mermaid
classDiagram
    class BaseEntity {
        <<abstract>>
        +Guid Id
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +DateTime? DeletedAt
    }

    class Employee {
        +string FirstName
        +string LastName
        +string Email
        +List~LeaveRequest~ LeaveRequests
    }

    class LeaveRequest {
        +DateTime StartDate
        +DateTime EndDate
        +LeaveStatus Status
        +Guid EmployeeId
        +Approve()
        +Reject(string reason)
    }

    class LeaveStatus {
        <<enumeration>>
        Pending
        Approved
        Rejected
        Cancelled
    }

    BaseEntity <|-- Employee
    BaseEntity <|-- LeaveRequest
    Employee "1" --> "*" LeaveRequest : has
    LeaveRequest --> LeaveStatus : status
```

**Conventions:**
- Use `<<abstract>>`, `<<interface>>`, `<<enumeration>>` stereotypes
- Show visibility: `+` public, `-` private, `#` protected
- Use generics with `~Type~` syntax
- Relationship arrows: `<|--` inheritance, `-->` association, `..>` dependency

---

### ER Diagram (Database Schema)

Use for database design and entity relationships.

```mermaid
erDiagram
    EMPLOYEE {
        uuid id PK
        string first_name
        string last_name
        string email UK
        datetime created_at
        datetime deleted_at
    }

    LEAVE_REQUEST {
        uuid id PK
        uuid employee_id FK
        datetime start_date
        datetime end_date
        string status
        string reason
        datetime created_at
    }

    LEAVE_TYPE {
        uuid id PK
        string name UK
        int default_days
        boolean is_paid
    }

    EMPLOYEE ||--o{ LEAVE_REQUEST : "submits"
    LEAVE_REQUEST }o--|| LEAVE_TYPE : "has type"
```

**Conventions:**
- Mark keys: `PK` (primary), `FK` (foreign), `UK` (unique)
- Cardinality: `||` one, `o{` zero-or-many, `|{` one-or-many
- Use snake_case for column names

---

### State Diagram (Workflows)

Use for state machines and lifecycle management.

```mermaid
stateDiagram-v2
    [*] --> Draft: Create request

    Draft --> Pending: Submit
    Draft --> Cancelled: Cancel

    Pending --> Approved: Approve
    Pending --> Rejected: Reject
    Pending --> Cancelled: Cancel

    Approved --> [*]
    Rejected --> Draft: Revise
    Rejected --> [*]
    Cancelled --> [*]

    note right of Pending
        Awaiting manager approval
    end note
```

**Conventions:**
- Use `[*]` for start/end states
- Add transitions with labels: `State1 --> State2: Action`
- Use `note` for additional context
- Group states with `state "name" as alias { }`

---

### Git Graph (Branching Strategy)

Use for visualizing Git workflows and release strategies.

```mermaid
gitGraph
    commit id: "init"
    branch develop
    checkout develop
    commit id: "feat: add models"
    branch feature/leave-approval
    commit id: "feat: approval logic"
    commit id: "test: add tests"
    checkout develop
    merge feature/leave-approval
    checkout main
    merge develop tag: "v1.0.0"
```

---

### Pie Chart (Statistics)

Use for showing distributions and proportions.

```mermaid
pie showData
    title Leave Requests by Status
    "Approved" : 45
    "Pending" : 25
    "Rejected" : 15
    "Cancelled" : 15
```

---

### Gantt Chart (Planning)

Use for project timelines and task scheduling.

```mermaid
gantt
    title Sprint Planning
    dateFormat YYYY-MM-DD
    section Backend
        Leave API           :done, api, 2024-01-01, 5d
        Approval workflow   :active, approval, after api, 3d
        Integration tests   :test, after approval, 2d
    section Frontend
        Leave form          :form, 2024-01-01, 4d
        Dashboard           :after form, 3d
```

---

## 🧱 Naming Conventions

| Layer | Pattern | Examples |
|-------|---------|----------|
| API | `HTTP_METHOD /path` | `POST /api/leaves` |
| Application | `XxxCommand`, `XxxHandler`, `XxxValidator` | `ApproveLeaveCommand` |
| Domain | `Entity`, `ValueObject`, `Enum` | `Employee`, `LeaveStatus` |
| Database | `snake_case` tables/columns | `leave_request`, `employee_id` |

---

## ⚠️ Common Pitfalls to Avoid

* ❌ Using `{}` without quotes in labels
* ❌ Overcrowded diagrams (max ~12-15 nodes per view)
* ❌ Mixing diagram types inappropriately
* ❌ Missing `autonumber` in complex sequence diagrams
* ❌ Vague labels like `Service` or `Manager` without context
* ❌ Forgetting relationship cardinality in ER diagrams

---

## 🧪 Validation Checklist

Before returning a Mermaid diagram:

* [ ] Parses correctly in [Mermaid Live Editor](https://mermaid.live)
* [ ] All special characters are quoted
* [ ] Diagram type matches the use case
* [ ] Naming is consistent with conventions
* [ ] Not overcrowded (split if needed)
* [ ] Arrows/relationships make logical sense

---

## ✨ Optional Enhancements

If requested:
* Add comments using `%%`
* Use `classDef` for custom styling
* Add `click` handlers for interactive diagrams
* Use `rect` backgrounds in sequence diagrams for grouping
