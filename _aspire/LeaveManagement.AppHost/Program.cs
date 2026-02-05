var builder = DistributedApplication.CreateBuilder(args);

<<<<<<<< HEAD:_aspire/LeaveManagement.AppHost/AppHost.cs
builder.AddProject<Projects.LeaveManagement_API>("leavemanagement-api");
========
builder.AddProject<Projects.LeaveManagement_API>("leavemanagement.api");
builder.AddProject<Projects.LeaveManagement_McpServer>("leavemanagement.mcpserver");
>>>>>>>> 443523f (feat(mcpServer): phase 1 and 2):_aspire/LeaveManagement.AppHost/Program.cs

builder.Build().Run();
