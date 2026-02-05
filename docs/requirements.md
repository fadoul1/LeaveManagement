# Requirements Document

## Introduction

This document defines the requirements for extending the existing LeaveManagement .NET Web API by implementing a Model Context Protocol (MCP) Server layer. The MCP Server will enable AI agents (such as Claude Code) to discover and execute existing business logic as native "Tools" through a standardized protocol.

The integration will expose the Employee and Leave management capabilities—including CRUD operations, validation, and queries—to AI clients via stdio-based communication, allowing intelligent automation and natural language interaction with the system.

---

## Requirements

### 1. MCP Server Infrastructure

**User Story:**
> As a developer, I want a dedicated MCP Server entry point so that AI agents can connect and communicate with the LeaveManagement system through the Model Context Protocol.

**Acceptance Criteria:**

- WHEN the MCP Server is started THEN the system SHALL initialize a stdio-based transport layer using ModelContextProtocol.NET SDK.
- WHEN the MCP Server is running THEN the system SHALL accept incoming MCP protocol messages via standard input.
- WHEN the MCP Server processes requests THEN the system SHALL respond via standard output in valid MCP JSON format.
- WHEN the MCP Server encounters a fatal error during startup THEN the system SHALL log the error and exit with a non-zero exit code.
- WHEN the MCP Server is configured THEN the system SHALL read connection strings and settings from environment variables or appsettings.json.

---

### 2. Tool Discovery (ListTools)

**User Story:**
> As an AI agent, I want to discover all available tools so that I can understand what operations are available in the LeaveManagement system.

**Acceptance Criteria:**

- WHEN a ListTools request is received THEN the system SHALL return a list of all exposed MCP Tools.
- WHEN listing tools THEN each tool definition SHALL include a unique name, description, and JSON Schema for input parameters.
- WHEN listing tools THEN the system SHALL expose Employee operations: CreateEmployee, UpdateEmployee, DeleteEmployee, GetAllEmployees, GetEmployeeById.
- WHEN listing tools THEN the system SHALL expose Leave operations: CreateLeave, GetAllLeaves, GetLeavesByEmployeeId.
- WHEN listing tools THEN complex DTOs SHALL be represented with accurate JSON Schema definitions including required fields and validation constraints.
- WHEN listing tools THEN enum parameters (LeaveType, LeaveStatus) SHALL include allowed values in their schema definitions.

---

### 3. Tool Execution (CallTool)

**User Story:**
> As an AI agent, I want to execute tools by name with provided parameters so that I can perform business operations on the LeaveManagement system.

**Acceptance Criteria:**

- WHEN a CallTool request is received with a valid tool name and parameters THEN the system SHALL route the request to the appropriate MediatR handler.
- WHEN executing CreateEmployee THEN the system SHALL validate input, create the employee record, and return EmployeeResponse.
- WHEN executing UpdateEmployee THEN the system SHALL validate input, update the existing employee record, and return EmployeeResponse.
- WHEN executing DeleteEmployee THEN the system SHALL soft-delete the employee (set DeletedAt) and return success confirmation.
- WHEN executing GetAllEmployees THEN the system SHALL return a list of all non-deleted employees as EmployeeResponse objects.
- WHEN executing GetEmployeeById THEN the system SHALL return the employee matching the ID or an error if not found.
- WHEN executing CreateLeave THEN the system SHALL validate input, create the leave record, and return LeaveResponse.
- WHEN executing GetAllLeaves THEN the system SHALL return a list of all leaves as LeaveResponse objects.
- WHEN executing GetLeavesByEmployeeId THEN the system SHALL return all leaves for the specified employee.
- WHEN a tool returns a response THEN the system SHALL serialize the response as JSON content in the MCP result format.

---

### 4. Input Validation

**User Story:**
> As an AI agent, I want clear validation feedback so that I can correct my inputs when they don't meet business rules.

**Acceptance Criteria:**

- WHEN CreateEmployee is called with invalid email format THEN the system SHALL return a validation error specifying the email constraint.
- WHEN CreateEmployee is called with FirstName or LastName shorter than 2 characters THEN the system SHALL return a validation error.
- WHEN CreateEmployee is called with PhoneNumber shorter than 8 characters THEN the system SHALL return a validation error.
- WHEN CreateLeave is called with EmployeeId less than or equal to 0 THEN the system SHALL return a validation error.
- WHEN CreateLeave is called with StartDate or EndDate not greater than today THEN the system SHALL return a validation error.
- WHEN validation fails THEN the response SHALL include Success=false and ValidationErrors containing all failed constraints.
- WHEN multiple validation rules fail THEN all violations SHALL be included in the ValidationErrors field.

---

### 5. Error Handling

**User Story:**
> As an AI agent, I want meaningful error responses so that I can understand why an operation failed and take appropriate action.

**Acceptance Criteria:**

- WHEN a CallTool request references a non-existent tool THEN the system SHALL return an MCP error with code "MethodNotFound" and descriptive message.
- WHEN a CallTool request has malformed parameters (invalid JSON) THEN the system SHALL return an MCP error with code "InvalidParams" and describe the parsing issue.
- WHEN a required parameter is missing THEN the system SHALL return an MCP error indicating which parameter is missing.
- WHEN GetEmployeeById is called with a non-existent ID THEN the system SHALL return an error indicating the employee was not found.
- WHEN a database operation fails THEN the system SHALL return an MCP error with code "InternalError" without exposing sensitive details.
- WHEN an unexpected exception occurs THEN the system SHALL log the full exception details and return a generic error to the client.

---

### 6. JSON Schema Generation

**User Story:**
> As an AI agent, I want accurate JSON Schemas for tool parameters so that I can construct valid requests without trial and error.

**Acceptance Criteria:**

- WHEN generating schema for CreateEmployeeCommand THEN the schema SHALL include FirstName, LastName, Email, PhoneNumber as required string properties.
- WHEN generating schema for UpdateEmployeeCommand THEN the schema SHALL include EmployeeId as a required integer and other employee fields.
- WHEN generating schema for DeleteEmployeeCommand THEN the schema SHALL include EmployeeId as a required integer.
- WHEN generating schema for CreateLeaveCommand THEN the schema SHALL include Type (enum), Status (enum), StartDate, EndDate, Reason, and EmployeeId.
- WHEN generating schema for enum types THEN the schema SHALL use "enum" keyword with all valid string values.
- WHEN generating schema for date fields THEN the schema SHALL specify "format": "date-time" for DateTime properties.
- WHEN generating schemas THEN validation constraints (minLength, minimum) SHALL be reflected in the JSON Schema where applicable.

---

### 7. Resource Exposure (Context)

**User Story:**
> As an AI agent, I want to read system resources so that I can gain context before executing operations.

**Acceptance Criteria:**

- WHEN a ListResources request is received THEN the system SHALL return available context resources.
- WHEN the "schema://database" resource is requested THEN the system SHALL return the database schema information (tables, columns, relationships).
- WHEN the "config://enums" resource is requested THEN the system SHALL return all enum definitions (LeaveType, LeaveStatus) with their values and descriptions.
- WHEN the "status://health" resource is requested THEN the system SHALL return the current health status of the API and database connection.
- WHEN reading a resource THEN the response SHALL include the resource URI, MIME type, and content.
- WHEN a non-existent resource is requested THEN the system SHALL return an appropriate error indicating the resource is not found.

---

### 8. Service Integration

**User Story:**
> As a developer, I want the MCP Server to integrate with existing services so that it reuses the established business logic without duplication.

**Acceptance Criteria:**

- WHEN the MCP Server starts THEN the system SHALL configure dependency injection with all Application layer services.
- WHEN the MCP Server starts THEN the system SHALL configure the EF Core DbContext with the production connection string.
- WHEN a tool is executed THEN the system SHALL resolve handlers from the DI container.
- WHEN executing tools THEN the system SHALL use the existing MediatR pipeline including validation behaviors.
- WHEN executing tools THEN the system SHALL share the same DbContext and repositories as the Web API.

---

### 9. Configuration and Security

**User Story:**
> As a developer, I want secure configuration management so that sensitive data like connection strings are protected.

**Acceptance Criteria:**

- WHEN the MCP Server is configured THEN connection strings SHALL be read from environment variables first, then appsettings.json as fallback.
- WHEN sensitive configuration is loaded THEN the system SHALL NOT log connection strings or credentials.
- WHEN the MCP Server runs locally THEN the system SHALL execute with the same permissions as the host process.
- WHEN invalid configuration is detected THEN the system SHALL fail fast with a clear error message indicating the missing configuration.

---

### 10. Logging and Diagnostics

**User Story:**
> As a developer, I want comprehensive logging so that I can diagnose issues with MCP communication and tool execution.

**Acceptance Criteria:**

- WHEN the MCP Server starts THEN the system SHALL log the startup configuration and available tools count.
- WHEN a tool is called THEN the system SHALL log the tool name and execution duration.
- WHEN a tool execution fails THEN the system SHALL log the error details including stack trace.
- WHEN MCP protocol errors occur THEN the system SHALL log the raw message content for debugging.
- WHEN logging THEN the system SHALL NOT log sensitive parameter values (passwords, tokens).
- WHEN logging THEN the system SHALL use structured logging compatible with the existing Aspire telemetry setup.

---

### 11. Testing Support

**User Story:**
> As a developer, I want the MCP Server components to be testable so that I can verify correct behavior in automated tests.

**Acceptance Criteria:**

- WHEN testing tool execution THEN it SHALL be possible to mock the MediatR handlers.
- WHEN testing MCP protocol handling THEN it SHALL be possible to simulate MCP requests without stdio.
- WHEN testing error scenarios THEN it SHALL be possible to inject failures in the handler pipeline.
- WHEN testing schema generation THEN the schemas SHALL match the expected JSON Schema format.
- WHEN integration testing THEN it SHALL be possible to use an in-memory database as with existing tests.

---

### 12. Documentation

**User Story:**
> As a developer, I want documentation on MCP integration so that I can understand, maintain, and extend the system.

**Acceptance Criteria:**

- WHEN the MCP Server is implemented THEN the CLAUDE.md file SHALL be updated with MCP-specific commands and configuration.
- WHEN tools are defined THEN each tool SHALL have a description explaining its purpose and expected parameters.
- WHEN the MCP Server is deployed THEN documentation SHALL include instructions for running the server with Claude Code.
- WHEN extending the system THEN documentation SHALL explain how to add new tools from existing handlers.
