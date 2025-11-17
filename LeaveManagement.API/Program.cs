using LeaveManagement.API;
using LeaveManagement.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder
       .ConfigureServices()
       .ConfigurePipeline();

await app.CreateDatabaseAsync();

app.Run();
