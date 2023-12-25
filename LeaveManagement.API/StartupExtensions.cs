using LeaveManagement.Persistence.Data;
using LeaveManagement.Persistence;
using LeaveManagement.Application;
using Microsoft.OpenApi.Models;

namespace LeaveManagement.API;

public static class StartupExtensions
{
    public static WebApplication ConfigureServices(
            this WebApplicationBuilder builder)
    {
        AddSwagger(builder.Services);

        builder.Services.AddApplicationServices();

        builder.Services.AddPersistenceServices(builder.Configuration);

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        return builder.Build();

    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Leave Management API");
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors();

        app.MapControllers();

        return app;

    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Leave Management API",

            });

        });
    }

    public static async Task CreateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var context = scope.ServiceProvider.GetService<ApplicationContext>();
            if (context is not null)
            {
                await context.Database.EnsureCreatedAsync();
            }
        }
        catch (Exception ex)
        {
            //var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
           // logger.LogError(ex, "An error occurred while creating the database.");
        }
    }
}
