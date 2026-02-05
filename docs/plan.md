# Implementation Plan

## Overview

This plan outlines the implementation strategy for adding an MCP (Model Context Protocol) Server layer to the LeaveManagement .NET Web API. The implementation follows the existing Clean Architecture patterns and integrates with the CQRS infrastructure.

---

## Phase 1: Foundation & Infrastructure

### P1.1 - Project Setup and Dependencies
**Priority:** High
**Requirements:** [Req 1](#1-mcp-server-infrastructure), [Req 8](#8-service-integration), [Req 9](#9-configuration-and-security)

Create the MCP Server project and configure dependencies:
- Create new `LeaveManagement.McpServer` console project
- Add ModelContextProtocol.NET NuGet package
- Configure project references to Application and Persistence layers
- Set up project in solution file and Aspire orchestration

### P1.2 - Service Configuration
**Priority:** High
**Requirements:** [Req 1](#1-mcp-server-infrastructure), [Req 8](#8-service-integration), [Req 9](#9-configuration-and-security)

Configure dependency injection and service registration:
- Reuse existing `ApplicationServiceRegistration` from Application layer
- Reuse existing `PersistenceServiceRegistration` from Persistence layer
- Configure connection string loading from environment/appsettings
- Set up logging with structured logging support

### P1.3 - MCP Server Entry Point
**Priority:** High
**Requirements:** [Req 1](#1-mcp-server-infrastructure)

Implement the main entry point:
- Create Program.cs with MCP Server initialization
- Configure stdio transport layer
- Implement graceful shutdown handling
- Add startup validation and error reporting

---

## Phase 2: Tool Discovery & Schema Generation

### P2.1 - Tool Registry Architecture
**Priority:** High
**Requirements:** [Req 2](#2-tool-discovery-listtools), [Req 6](#6-json-schema-generation)

Design and implement the tool registration system:
- Create `IMcpToolDefinition` interface for tool metadata
- Create `McpToolRegistry` to manage tool registrations
- Implement auto-discovery of tools from MediatR handlers

### P2.2 - JSON Schema Generator
**Priority:** High
**Requirements:** [Req 6](#6-json-schema-generation)

Implement JSON Schema generation for command/query types:
- Create schema generator for C# types to JSON Schema
- Handle primitive types (string, int, long, DateTime)
- Handle enum types with allowed values
- Handle validation constraints (minLength, minimum, required)
- Handle nested complex types

### P2.3 - Employee Tool Definitions
**Priority:** High
**Requirements:** [Req 2](#2-tool-discovery-listtools), [Req 6](#6-json-schema-generation)

Define MCP Tools for Employee operations:
- CreateEmployee tool with schema from CreateEmployeeCommand
- UpdateEmployee tool with schema from UpdateEmployeeCommand
- DeleteEmployee tool with schema from DeleteEmployeeCommand
- GetAllEmployees tool (no parameters)
- GetEmployeeById tool with EmployeeId parameter

### P2.4 - Leave Tool Definitions
**Priority:** High
**Requirements:** [Req 2](#2-tool-discovery-listtools), [Req 6](#6-json-schema-generation)

Define MCP Tools for Leave operations:
- CreateLeave tool with schema from CreateLeaveCommand
- GetAllLeaves tool (no parameters)
- GetLeavesByEmployeeId tool with EmployeeId parameter

### P2.5 - ListTools Handler
**Priority:** High
**Requirements:** [Req 2](#2-tool-discovery-listtools)

Implement the MCP ListTools protocol handler:
- Query McpToolRegistry for all registered tools
- Format response according to MCP specification
- Include tool names, descriptions, and input schemas

---

## Phase 3: Tool Execution

### P3.1 - CallTool Router
**Priority:** High
**Requirements:** [Req 3](#3-tool-execution-calltool)

Implement the tool execution router:
- Parse incoming CallTool requests
- Resolve tool by name from registry
- Deserialize parameters to command/query types
- Dispatch to appropriate executor

### P3.2 - MediatR Integration
**Priority:** High
**Requirements:** [Req 3](#3-tool-execution-calltool), [Req 8](#8-service-integration)

Integrate tool execution with MediatR pipeline:
- Create `McpToolExecutor` that sends commands/queries to MediatR
- Ensure validation behaviors are invoked
- Capture and format handler responses

### P3.3 - Employee Tool Executors
**Priority:** High
**Requirements:** [Req 3](#3-tool-execution-calltool)

Implement executors for Employee tools:
- CreateEmployee: Dispatch CreateEmployeeCommand, return EmployeeResponse
- UpdateEmployee: Dispatch UpdateEmployeeCommand, return EmployeeResponse
- DeleteEmployee: Dispatch DeleteEmployeeCommand, return success confirmation
- GetAllEmployees: Dispatch GetEmployeesListQuery, return list
- GetEmployeeById: Dispatch GetEmployeeByIdQuery, return single employee

### P3.4 - Leave Tool Executors
**Priority:** High
**Requirements:** [Req 3](#3-tool-execution-calltool)

Implement executors for Leave tools:
- CreateLeave: Dispatch CreateLeaveCommand, return LeaveResponse
- GetAllLeaves: Dispatch GetLeavesListQuery, return list
- GetLeavesByEmployeeId: Dispatch GetLeavesByEmployeeIdQuery, return list

### P3.5 - Response Serialization
**Priority:** High
**Requirements:** [Req 3](#3-tool-execution-calltool)

Implement response formatting:
- Serialize handler responses to JSON
- Format as MCP content blocks
- Handle null/empty responses appropriately

---

## Phase 4: Validation & Error Handling

### P4.1 - Validation Error Translation
**Priority:** High
**Requirements:** [Req 4](#4-input-validation), [Req 5](#5-error-handling)

Translate FluentValidation results to MCP format:
- Capture ValidationErrors from BaseResponse
- Format as user-friendly MCP error content
- Include all validation failures in response

### P4.2 - Parameter Validation
**Priority:** High
**Requirements:** [Req 4](#4-input-validation), [Req 5](#5-error-handling)

Implement parameter validation before execution:
- Validate required parameters are present
- Validate parameter types match schema
- Return InvalidParams error for malformed input

### P4.3 - Exception Handling
**Priority:** High
**Requirements:** [Req 5](#5-error-handling)

Implement comprehensive exception handling:
- Catch and translate .NET exceptions to MCP errors
- MethodNotFound for unknown tools
- InvalidParams for deserialization failures
- InternalError for unexpected exceptions
- Ensure sensitive details are not exposed

### P4.4 - Not Found Handling
**Priority:** Medium
**Requirements:** [Req 5](#5-error-handling)

Handle resource-not-found scenarios:
- Detect when GetEmployeeById returns null/empty
- Return appropriate "not found" error response
- Include the ID that was not found in the message

---

## Phase 5: Resources (Context)

### P5.1 - Resource Registry
**Priority:** Medium
**Requirements:** [Req 7](#7-resource-exposure-context)

Implement resource management system:
- Create `IMcpResource` interface
- Create `McpResourceRegistry` for registration
- Implement ListResources handler

### P5.2 - Database Schema Resource
**Priority:** Medium
**Requirements:** [Req 7](#7-resource-exposure-context)

Expose database schema as readable resource:
- Query EF Core model metadata
- Format as table/column definitions
- Include relationships and constraints

### P5.3 - Enum Definitions Resource
**Priority:** Medium
**Requirements:** [Req 7](#7-resource-exposure-context)

Expose enum values as readable resource:
- List LeaveTypeEnum values with descriptions
- List LeaveStatusEnum values with descriptions
- Format as JSON for easy consumption

### P5.4 - Health Status Resource
**Priority:** Low
**Requirements:** [Req 7](#7-resource-exposure-context)

Expose system health as readable resource:
- Check database connectivity
- Report API health status
- Include version information

### P5.5 - ReadResource Handler
**Priority:** Medium
**Requirements:** [Req 7](#7-resource-exposure-context)

Implement the MCP ReadResource handler:
- Parse resource URI from request
- Resolve resource from registry
- Return content with appropriate MIME type

---

## Phase 6: Logging & Diagnostics

### P6.1 - Structured Logging Setup
**Priority:** Medium
**Requirements:** [Req 10](#10-logging-and-diagnostics)

Configure logging infrastructure:
- Integrate with Aspire ServiceDefaults
- Configure log levels for MCP operations
- Set up console and file logging

### P6.2 - Request/Response Logging
**Priority:** Medium
**Requirements:** [Req 10](#10-logging-and-diagnostics)

Implement MCP protocol logging:
- Log incoming request method and tool name
- Log execution duration for each tool call
- Avoid logging sensitive parameter values

### P6.3 - Error Logging
**Priority:** High
**Requirements:** [Req 10](#10-logging-and-diagnostics)

Implement error and exception logging:
- Log full stack traces for exceptions
- Log validation failures with context
- Log protocol-level errors

---

## Phase 7: Testing

### P7.1 - Unit Test Infrastructure
**Priority:** High
**Requirements:** [Req 11](#11-testing-support)

Set up unit testing project:
- Create `LeaveManagement.McpServer.Tests` project
- Configure xUnit and mocking frameworks
- Set up test fixtures

### P7.2 - Schema Generation Tests
**Priority:** Medium
**Requirements:** [Req 11](#11-testing-support)

Test JSON Schema generation:
- Verify CreateEmployeeCommand schema
- Verify CreateLeaveCommand schema with enums
- Verify validation constraints in schemas

### P7.3 - Tool Execution Tests
**Priority:** High
**Requirements:** [Req 11](#11-testing-support)

Test tool execution flow:
- Mock MediatR for isolated testing
- Verify correct handler dispatch
- Verify response formatting

### P7.4 - Error Handling Tests
**Priority:** Medium
**Requirements:** [Req 11](#11-testing-support)

Test error scenarios:
- Unknown tool returns MethodNotFound
- Invalid params returns InvalidParams
- Validation failure returns proper error

### P7.5 - Integration Tests
**Priority:** Medium
**Requirements:** [Req 11](#11-testing-support)

End-to-end integration tests:
- Test full tool execution with in-memory database
- Test resource reading
- Test ListTools response

---

## Phase 8: Documentation & Finalization

### P8.1 - CLAUDE.md Updates
**Priority:** High
**Requirements:** [Req 12](#12-documentation)

Update project documentation:
- Add MCP Server run commands
- Document tool names and purposes
- Add troubleshooting guide

### P8.2 - Tool Documentation
**Priority:** Medium
**Requirements:** [Req 12](#12-documentation)

Document each MCP tool:
- Purpose and use cases
- Parameter descriptions
- Example requests/responses

### P8.3 - Extension Guide
**Priority:** Low
**Requirements:** [Req 12](#12-documentation)

Create developer extension guide:
- How to add new tools from handlers
- How to add new resources
- Best practices and patterns

---

## Implementation Order Summary

| Phase | Priority | Dependencies |
|-------|----------|--------------|
| Phase 1: Foundation | High | None |
| Phase 2: Tool Discovery | High | Phase 1 |
| Phase 3: Tool Execution | High | Phase 2 |
| Phase 4: Validation/Errors | High | Phase 3 |
| Phase 6.3: Error Logging | High | Phase 4 |
| Phase 7.1: Test Infrastructure | High | Phase 1 |
| Phase 7.3: Tool Execution Tests | High | Phase 3 |
| Phase 8.1: CLAUDE.md | High | Phase 4 |
| Phase 5: Resources | Medium | Phase 3 |
| Phase 6.1-6.2: Logging | Medium | Phase 3 |
| Phase 7.2, 7.4, 7.5: Tests | Medium | Phase 4 |
| Phase 8.2-8.3: Documentation | Low | Phase 4 |
