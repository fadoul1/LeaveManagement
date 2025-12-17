var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LeaveManagement_API>("leavemanagement-api");

builder.Build().Run();
