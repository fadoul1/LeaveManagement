# Technical Task List

## Phase 1: Foundation & Infrastructure

### 1.1 Project Setup
> Plan: [P1.1](#p11---project-setup-and-dependencies) | Requirements: [Req 1](requirements.md#1-mcp-server-infrastructure), [Req 8](requirements.md#8-service-integration)

- [x] **T1.1.1** Create `src/LeaveManagement.McpServer/` console application project targeting .NET 8
- [x] **T1.1.2** Add `ModelContextProtocol` NuGet package to McpServer project
- [x] **T1.1.3** Add project reference to `LeaveManagement.Application`
- [x] **T1.1.4** Add project reference to `LeaveManagement.Persistence`
- [x] **T1.1.5** Add project reference to `LeaveManagement.ServiceDefaults`
- [x] **T1.1.6** Add McpServer project to `LeaveManagement.sln` solution file
- [x] **T1.1.7** Add McpServer to Aspire AppHost orchestration in `LeaveManagement.AppHost`

### 1.2 Service Configuration
> Plan: [P1.2](#p12---service-configuration) | Requirements: [Req 8](requirements.md#8-service-integration), [Req 9](requirements.md#9-configuration-and-security)

- [x] **T1.2.1** Create `appsettings.json` with connection string configuration
- [x] **T1.2.2** Create `appsettings.Development.json` for local development settings
- [x] **T1.2.3** Create `ServiceCollectionExtensions.cs` for MCP-specific service registration (handled in Program.cs)
- [x] **T1.2.4** Register Application layer services using existing `ApplicationServiceRegistration`
- [x] **T1.2.5** Register Persistence layer services using existing `PersistenceServiceRegistration`
- [x] **T1.2.6** Configure `IConfiguration` to read from environment variables with appsettings fallback
- [ ] **T1.2.7** Add ServiceDefaults for OpenTelemetry and structured logging

### 1.3 MCP Server Entry Point
> Plan: [P1.3](#p13---mcp-server-entry-point) | Requirements: [Req 1](requirements.md#1-mcp-server-infrastructure)

- [x] **T1.3.1** Create `Program.cs` with host builder configuration
- [x] **T1.3.2** Initialize MCP Server with stdio transport using `McpServerBuilder`
- [x] **T1.3.3** Configure cancellation token for graceful shutdown (SIGINT/SIGTERM)
- [x] **T1.3.4** Add startup validation to verify database connection
- [x] **T1.3.5** Implement error handling for fatal startup failures with non-zero exit code
- [x] **T1.3.6** Log server startup with available tools count

---

## Phase 2: Tool Discovery & Schema Generation

### 2.1 Tool Registry Architecture
> Plan: [P2.1](#p21---tool-registry-architecture) | Requirements: [Req 2](requirements.md#2-tool-discovery-listtools)
> **Note:** Tasks T2.1.1-T2.1.6 handled automatically by ModelContextProtocol SDK via `[McpServerToolType]` attribute and `WithToolsFromAssembly()`

- [x] **T2.1.1** Create `Contracts/IMcpTool.cs` interface (SDK provides via attributes)
- [x] **T2.1.2** Create `Services/McpToolRegistry.cs` class (SDK built-in)
- [x] **T2.1.3** Implement `RegisterTool<T>()` method (SDK built-in)
- [x] **T2.1.4** Implement `GetAllTools()` method (SDK built-in)
- [x] **T2.1.5** Implement `GetTool(string name)` method (SDK built-in)
- [x] **T2.1.6** Register McpToolRegistry as singleton (SDK built-in)

### 2.2 JSON Schema Generator
> Plan: [P2.2](#p22---json-schema-generator) | Requirements: [Req 6](requirements.md#6-json-schema-generation)
> **Note:** JSON Schema generation handled automatically by SDK from method parameters with `[Description]` attributes

- [x] **T2.2.1** Create `Services/JsonSchemaGenerator.cs` class (SDK built-in)
- [x] **T2.2.2** Implement schema generation for primitive types (SDK built-in)
- [x] **T2.2.3** Implement schema generation for DateTime (SDK built-in)
- [x] **T2.2.4** Implement schema generation for enum types (handled via string params)
- [x] **T2.2.5** Implement detection of required properties (SDK built-in)
- [ ] **T2.2.6** Implement `minLength` constraint (documented in tool description)
- [ ] **T2.2.7** Implement `minimum` constraint (documented in tool description)
- [x] **T2.2.8** Implement nested object schema generation (not needed - flat params)
- [x] **T2.2.9** Add caching of generated schemas (SDK built-in)

### 2.3 Employee Tool Definitions
> Plan: [P2.3](#p23---employee-tool-definitions) | Requirements: [Req 2](requirements.md#2-tool-discovery-listtools), [Req 6](requirements.md#6-json-schema-generation)
> **Implemented:** `Tools/EmployeeTools.cs` with `[McpServerTool]` attributes

- [x] **T2.3.1** Create `Tools/CreateEmployeeTool.cs` (in EmployeeTools.cs)
- [x] **T2.3.2** Generate JSON Schema for CreateEmployeeCommand (auto from params)
- [x] **T2.3.3** Create `Tools/UpdateEmployeeTool.cs` (in EmployeeTools.cs)
- [x] **T2.3.4** Generate JSON Schema for UpdateEmployeeCommand (auto from params)
- [x] **T2.3.5** Create `Tools/DeleteEmployeeTool.cs` (in EmployeeTools.cs)
- [x] **T2.3.6** Generate JSON Schema for DeleteEmployeeCommand (auto from params)
- [x] **T2.3.7** Create `Tools/GetAllEmployeesTool.cs` (in EmployeeTools.cs)
- [x] **T2.3.8** Create `Tools/GetEmployeeByIdTool.cs` (in EmployeeTools.cs)
- [x] **T2.3.9** Generate JSON Schema for GetEmployeeByIdQuery (auto from params)

### 2.4 Leave Tool Definitions
> Plan: [P2.4](#p24---leave-tool-definitions) | Requirements: [Req 2](requirements.md#2-tool-discovery-listtools), [Req 6](requirements.md#6-json-schema-generation)
> **Implemented:** `Tools/LeaveTools.cs` with `[McpServerTool]` attributes

- [x] **T2.4.1** Create `Tools/CreateLeaveTool.cs` (in LeaveTools.cs)
- [x] **T2.4.2** Generate JSON Schema for CreateLeaveCommand (auto from params)
- [x] **T2.4.3** Create `Tools/GetAllLeavesTool.cs` (in LeaveTools.cs)
- [x] **T2.4.4** Create `Tools/GetLeavesByEmployeeIdTool.cs` (in LeaveTools.cs)
- [x] **T2.4.5** Generate JSON Schema for GetLeavesByEmployeeIdQuery (auto from params)
- [x] **T2.4.6** Create `GetLeaveEnumValues` helper tool for enum discovery

### 2.5 ListTools Handler
> Plan: [P2.5](#p25---listtools-handler) | Requirements: [Req 2](requirements.md#2-tool-discovery-listtools)
> **Note:** Handled automatically by SDK via `WithToolsFromAssembly()`

- [x] **T2.5.1** Create `Handlers/ListToolsHandler.cs` (SDK built-in)
- [x] **T2.5.2** Query McpToolRegistry (SDK built-in)
- [x] **T2.5.3** Format each tool as MCP ToolDefinition (SDK built-in)
- [x] **T2.5.4** Return ListToolsResult (SDK built-in)
- [x] **T2.5.5** Register all Employee tools (auto via WithToolsFromAssembly)
- [x] **T2.5.6** Register all Leave tools (auto via WithToolsFromAssembly)

---

## Phase 3: Tool Execution

### 3.1 CallTool Router
> Plan: [P3.1](#p31---calltool-router) | Requirements: [Req 3](requirements.md#3-tool-execution-calltool)

- [ ] **T3.1.1** Create `Handlers/CallToolHandler.cs` for MCP CallTool request
- [ ] **T3.1.2** Parse tool name from CallToolRequest
- [ ] **T3.1.3** Resolve tool instance from McpToolRegistry by name
- [ ] **T3.1.4** Deserialize JSON parameters to tool's input type
- [ ] **T3.1.5** Invoke tool's Execute method with deserialized parameters
- [ ] **T3.1.6** Return CallToolResult with execution output

### 3.2 MediatR Integration
> Plan: [P3.2](#p32---mediatr-integration) | Requirements: [Req 3](requirements.md#3-tool-execution-calltool), [Req 8](requirements.md#8-service-integration)

- [ ] **T3.2.1** Create `Services/McpToolExecutor.cs` base class for tool execution
- [ ] **T3.2.2** Inject `IMediator` into tool executor
- [ ] **T3.2.3** Create `ExecuteCommandAsync<TCommand, TResponse>()` helper method
- [ ] **T3.2.4** Create `ExecuteQueryAsync<TQuery, TResponse>()` helper method
- [ ] **T3.2.5** Ensure MediatR validation behaviors run before handler execution

### 3.3 Employee Tool Executors
> Plan: [P3.3](#p33---employee-tool-executors) | Requirements: [Req 3](requirements.md#3-tool-execution-calltool)

- [ ] **T3.3.1** Implement `CreateEmployeeTool.Execute()` - send CreateEmployeeCommand via MediatR
- [ ] **T3.3.2** Implement `UpdateEmployeeTool.Execute()` - send UpdateEmployeeCommand via MediatR
- [ ] **T3.3.3** Implement `DeleteEmployeeTool.Execute()` - send DeleteEmployeeCommand via MediatR
- [ ] **T3.3.4** Implement `GetAllEmployeesTool.Execute()` - send GetEmployeesListQuery via MediatR
- [ ] **T3.3.5** Implement `GetEmployeeByIdTool.Execute()` - send GetEmployeeByIdQuery via MediatR

### 3.4 Leave Tool Executors
> Plan: [P3.4](#p34---leave-tool-executors) | Requirements: [Req 3](requirements.md#3-tool-execution-calltool)

- [ ] **T3.4.1** Implement `CreateLeaveTool.Execute()` - send CreateLeaveCommand via MediatR
- [ ] **T3.4.2** Implement `GetAllLeavesTool.Execute()` - send GetLeavesListQuery via MediatR
- [ ] **T3.4.3** Implement `GetLeavesByEmployeeIdTool.Execute()` - send GetLeavesByEmployeeIdQuery via MediatR

### 3.5 Response Serialization
> Plan: [P3.5](#p35---response-serialization) | Requirements: [Req 3](requirements.md#3-tool-execution-calltool)

- [ ] **T3.5.1** Create `Services/McpResponseSerializer.cs` for response formatting
- [ ] **T3.5.2** Implement JSON serialization of EmployeeResponse to MCP content
- [ ] **T3.5.3** Implement JSON serialization of LeaveResponse to MCP content
- [ ] **T3.5.4** Implement serialization of List<T> responses
- [ ] **T3.5.5** Handle null responses with appropriate empty content
- [ ] **T3.5.6** Set content type as "application/json" for all tool responses

---

## Phase 4: Validation & Error Handling

### 4.1 Validation Error Translation
> Plan: [P4.1](#p41---validation-error-translation) | Requirements: [Req 4](requirements.md#4-input-validation), [Req 5](requirements.md#5-error-handling)

- [ ] **T4.1.1** Create `Services/ValidationErrorTranslator.cs` class
- [ ] **T4.1.2** Detect validation failures from BaseResponse.Success == false
- [ ] **T4.1.3** Parse ValidationErrors string into structured error list
- [ ] **T4.1.4** Format validation errors as MCP tool result with isError=true
- [ ] **T4.1.5** Include all validation failure messages in error content

### 4.2 Parameter Validation
> Plan: [P4.2](#p42---parameter-validation) | Requirements: [Req 4](requirements.md#4-input-validation), [Req 5](requirements.md#5-error-handling)

- [ ] **T4.2.1** Validate required parameters are present in CallTool request
- [ ] **T4.2.2** Validate parameter JSON can be deserialized to expected type
- [ ] **T4.2.3** Return MCP InvalidParams error for missing required parameters
- [ ] **T4.2.4** Return MCP InvalidParams error for type mismatch errors
- [ ] **T4.2.5** Include parameter name and expected type in error message

### 4.3 Exception Handling
> Plan: [P4.3](#p43---exception-handling) | Requirements: [Req 5](requirements.md#5-error-handling)

- [ ] **T4.3.1** Create `Middleware/ExceptionHandlingMiddleware.cs` for global exception handling
- [ ] **T4.3.2** Catch `KeyNotFoundException` and return MethodNotFound MCP error
- [ ] **T4.3.3** Catch `JsonException` and return InvalidParams MCP error
- [ ] **T4.3.4** Catch `ArgumentException` and return InvalidParams MCP error
- [ ] **T4.3.5** Catch all other exceptions and return InternalError MCP error
- [ ] **T4.3.6** Ensure stack traces and internal details are not exposed to client
- [ ] **T4.3.7** Log full exception details server-side for debugging

### 4.4 Not Found Handling
> Plan: [P4.4](#p44---not-found-handling) | Requirements: [Req 5](requirements.md#5-error-handling)

- [ ] **T4.4.1** Detect empty/null response from GetEmployeeByIdQuery
- [ ] **T4.4.2** Return structured "employee not found" error with the searched ID
- [ ] **T4.4.3** Detect empty response from GetLeavesByEmployeeIdQuery for non-existent employee
- [ ] **T4.4.4** Return appropriate error message for leave queries on invalid employee

---

## Phase 5: Resources (Context)

### 5.1 Resource Registry
> Plan: [P5.1](#p51---resource-registry) | Requirements: [Req 7](requirements.md#7-resource-exposure-context)

- [ ] **T5.1.1** Create `Contracts/IMcpResource.cs` interface with Uri, Name, Description, MimeType
- [ ] **T5.1.2** Create `Services/McpResourceRegistry.cs` class for resource management
- [ ] **T5.1.3** Implement `RegisterResource()` method for resource registration
- [ ] **T5.1.4** Implement `GetAllResources()` method to list resources
- [ ] **T5.1.5** Implement `GetResource(string uri)` method to retrieve by URI
- [ ] **T5.1.6** Create `Handlers/ListResourcesHandler.cs` for MCP ListResources request

### 5.2 Database Schema Resource
> Plan: [P5.2](#p52---database-schema-resource) | Requirements: [Req 7](requirements.md#7-resource-exposure-context)

- [ ] **T5.2.1** Create `Resources/DatabaseSchemaResource.cs` implementing IMcpResource
- [ ] **T5.2.2** Inject `LeaveManagementDbContext` to access EF Core model
- [ ] **T5.2.3** Extract entity types (Employee, Leave) from DbContext
- [ ] **T5.2.4** Extract property names and types for each entity
- [ ] **T5.2.5** Extract relationships and foreign keys
- [ ] **T5.2.6** Format schema as JSON with tables, columns, and relationships
- [ ] **T5.2.7** Register resource with URI "schema://database"

### 5.3 Enum Definitions Resource
> Plan: [P5.3](#p53---enum-definitions-resource) | Requirements: [Req 7](requirements.md#7-resource-exposure-context)

- [ ] **T5.3.1** Create `Resources/EnumDefinitionsResource.cs` implementing IMcpResource
- [ ] **T5.3.2** Extract LeaveTypeEnum values (SickLeave, AnnualLeave, Other) with descriptions
- [ ] **T5.3.3** Extract LeaveStatusEnum values (InProgress, Finish) with descriptions
- [ ] **T5.3.4** Format as JSON with enum name, values array, and descriptions
- [ ] **T5.3.5** Register resource with URI "config://enums"

### 5.4 Health Status Resource
> Plan: [P5.4](#p54---health-status-resource) | Requirements: [Req 7](requirements.md#7-resource-exposure-context)

- [ ] **T5.4.1** Create `Resources/HealthStatusResource.cs` implementing IMcpResource
- [ ] **T5.4.2** Check database connectivity using DbContext.Database.CanConnectAsync()
- [ ] **T5.4.3** Include server version and startup time
- [ ] **T5.4.4** Format as JSON with status, database, and version information
- [ ] **T5.4.5** Register resource with URI "status://health"

### 5.5 ReadResource Handler
> Plan: [P5.5](#p55---readresource-handler) | Requirements: [Req 7](requirements.md#7-resource-exposure-context)

- [ ] **T5.5.1** Create `Handlers/ReadResourceHandler.cs` for MCP ReadResource request
- [ ] **T5.5.2** Parse resource URI from request
- [ ] **T5.5.3** Resolve resource from McpResourceRegistry
- [ ] **T5.5.4** Execute resource's content retrieval method
- [ ] **T5.5.5** Return ReadResourceResult with content and MIME type
- [ ] **T5.5.6** Return ResourceNotFound error for unknown URIs

---

## Phase 6: Logging & Diagnostics

### 6.1 Structured Logging Setup
> Plan: [P6.1](#p61---structured-logging-setup) | Requirements: [Req 10](requirements.md#10-logging-and-diagnostics)

- [ ] **T6.1.1** Configure ILogger<T> injection in MCP Server components
- [ ] **T6.1.2** Add ServiceDefaults for OpenTelemetry tracing
- [ ] **T6.1.3** Configure log level filtering (Information for MCP, Warning for EF)
- [ ] **T6.1.4** Ensure logs output to stderr (stdout reserved for MCP protocol)

### 6.2 Request/Response Logging
> Plan: [P6.2](#p62---requestresponse-logging) | Requirements: [Req 10](requirements.md#10-logging-and-diagnostics)

- [ ] **T6.2.1** Log ListTools requests with requesting client info
- [ ] **T6.2.2** Log CallTool requests with tool name
- [ ] **T6.2.3** Log execution duration using Stopwatch
- [ ] **T6.2.4** Create filter to exclude sensitive parameters from logs (passwords, tokens)
- [ ] **T6.2.5** Log ReadResource requests with resource URI

### 6.3 Error Logging
> Plan: [P6.3](#p63---error-logging) | Requirements: [Req 10](requirements.md#10-logging-and-diagnostics)

- [ ] **T6.3.1** Log validation failures with input context
- [ ] **T6.3.2** Log exceptions with full stack trace at Error level
- [ ] **T6.3.3** Log MCP protocol errors at Warning level
- [ ] **T6.3.4** Include correlation ID in all log entries for request tracing

---

## Phase 7: Testing

### 7.1 Unit Test Infrastructure
> Plan: [P7.1](#p71---unit-test-infrastructure) | Requirements: [Req 11](requirements.md#11-testing-support)

- [ ] **T7.1.1** Create `tests/LeaveManagement.McpServer.Tests/` xUnit test project
- [ ] **T7.1.2** Add project reference to McpServer project
- [ ] **T7.1.3** Add NSubstitute or Moq for mocking
- [ ] **T7.1.4** Add FluentAssertions for readable assertions
- [ ] **T7.1.5** Create `TestFixtures/McpTestFixture.cs` with DI container setup

### 7.2 Schema Generation Tests
> Plan: [P7.2](#p72---schema-generation-tests) | Requirements: [Req 11](requirements.md#11-testing-support)

- [ ] **T7.2.1** Test JsonSchemaGenerator produces valid JSON Schema for CreateEmployeeCommand
- [ ] **T7.2.2** Test schema includes required properties (FirstName, LastName, Email, PhoneNumber)
- [ ] **T7.2.3** Test schema includes minLength constraints
- [ ] **T7.2.4** Test schema for CreateLeaveCommand includes enum values for LeaveType
- [ ] **T7.2.5** Test schema for CreateLeaveCommand includes enum values for LeaveStatus
- [ ] **T7.2.6** Test schema for DateTime properties includes format: date-time

### 7.3 Tool Execution Tests
> Plan: [P7.3](#p73---tool-execution-tests) | Requirements: [Req 11](requirements.md#11-testing-support)

- [ ] **T7.3.1** Test CreateEmployeeTool dispatches CreateEmployeeCommand to MediatR
- [ ] **T7.3.2** Test CreateEmployeeTool returns EmployeeResponse JSON
- [ ] **T7.3.3** Test GetAllEmployeesTool returns list of employees
- [ ] **T7.3.4** Test GetEmployeeByIdTool returns single employee
- [ ] **T7.3.5** Test DeleteEmployeeTool dispatches DeleteEmployeeCommand
- [ ] **T7.3.6** Test CreateLeaveTool dispatches CreateLeaveCommand with enums
- [ ] **T7.3.7** Test GetLeavesByEmployeeIdTool returns filtered leaves

### 7.4 Error Handling Tests
> Plan: [P7.4](#p74---error-handling-tests) | Requirements: [Req 11](requirements.md#11-testing-support)

- [ ] **T7.4.1** Test CallTool with unknown tool name returns MethodNotFound error
- [ ] **T7.4.2** Test CallTool with malformed JSON returns InvalidParams error
- [ ] **T7.4.3** Test CallTool with missing required param returns InvalidParams error
- [ ] **T7.4.4** Test validation failure returns error with ValidationErrors content
- [ ] **T7.4.5** Test GetEmployeeById with non-existent ID returns not found error
- [ ] **T7.4.6** Test internal exception returns InternalError without exposing details

### 7.5 Integration Tests
> Plan: [P7.5](#p75---integration-tests) | Requirements: [Req 11](requirements.md#11-testing-support)

- [ ] **T7.5.1** Create integration test fixture with in-memory database
- [ ] **T7.5.2** Test full CreateEmployee flow from MCP request to database
- [ ] **T7.5.3** Test full GetAllEmployees returns seeded test data
- [ ] **T7.5.4** Test ListTools returns all 8 registered tools
- [ ] **T7.5.5** Test ListResources returns all 3 registered resources
- [ ] **T7.5.6** Test ReadResource for database schema returns valid JSON

---

## Phase 8: Documentation & Finalization

### 8.1 CLAUDE.md Updates
> Plan: [P8.1](#p81---claudemd-updates) | Requirements: [Req 12](requirements.md#12-documentation)

- [ ] **T8.1.1** Add MCP Server run command to Build and Run Commands section
- [ ] **T8.1.2** Document MCP Server architecture in Architecture section
- [ ] **T8.1.3** Add list of available MCP Tools with descriptions
- [ ] **T8.1.4** Add list of available MCP Resources with URIs
- [ ] **T8.1.5** Document Claude Code integration configuration
- [ ] **T8.1.6** Add troubleshooting section for common MCP issues

### 8.2 Tool Documentation
> Plan: [P8.2](#p82---tool-documentation) | Requirements: [Req 12](requirements.md#12-documentation)

- [ ] **T8.2.1** Document CreateEmployee tool: purpose, parameters, example
- [ ] **T8.2.2** Document UpdateEmployee tool: purpose, parameters, example
- [ ] **T8.2.3** Document DeleteEmployee tool: purpose, parameters, example
- [ ] **T8.2.4** Document GetAllEmployees tool: purpose, response format
- [ ] **T8.2.5** Document GetEmployeeById tool: purpose, parameters, example
- [ ] **T8.2.6** Document CreateLeave tool: purpose, parameters, enum values
- [ ] **T8.2.7** Document GetAllLeaves tool: purpose, response format
- [ ] **T8.2.8** Document GetLeavesByEmployeeId tool: purpose, parameters

### 8.3 Extension Guide
> Plan: [P8.3](#p83---extension-guide) | Requirements: [Req 12](requirements.md#12-documentation)

- [ ] **T8.3.1** Document how to add a new tool from existing MediatR handler
- [ ] **T8.3.2** Document how to add a new resource
- [ ] **T8.3.3** Document JSON Schema generation best practices
- [ ] **T8.3.4** Document error handling patterns
- [ ] **T8.3.5** Document testing patterns for new tools

---

## Task Summary

| Phase | Task Count | Priority |
|-------|------------|----------|
| Phase 1: Foundation | 20 tasks | High |
| Phase 2: Tool Discovery | 25 tasks | High |
| Phase 3: Tool Execution | 17 tasks | High |
| Phase 4: Validation/Errors | 16 tasks | High |
| Phase 5: Resources | 22 tasks | Medium |
| Phase 6: Logging | 12 tasks | Medium |
| Phase 7: Testing | 26 tasks | High/Medium |
| Phase 8: Documentation | 19 tasks | Medium/Low |
| **Total** | **157 tasks** | |
