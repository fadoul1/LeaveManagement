using LeaveManagement.Persistence.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.API.IntegrationTests.Support;

internal class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {            
            try
            {
                services.RemoveAll(typeof(DbContextOptions<ApplicationContext>));
                services.AddDbContext<ApplicationContext>(options =>
                {
                    options.UseInMemoryDatabase("LeaveManagementDbAPITest");
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                context.Database.EnsureCreated();
                //Utilities.InitializeEmployeesForTests(context);
            }
            catch (Exception ex)
            {
                // Ajoutez des journaux ici
                Console.WriteLine($"Error during database setup: {ex.Message}");
                throw new Exception($"An error occurred during database setup. Error: {ex.Message}");
            }
        });
    }

    public HttpClient GetAnonymousClient()
    {
        return CreateClient();
    }
}