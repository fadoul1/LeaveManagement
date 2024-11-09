using LeaveManagement.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.ConfigureServices().ConfigurePipeline();

await app.CreateDatabaseAsync();

app.Run();

public partial class Program { }
