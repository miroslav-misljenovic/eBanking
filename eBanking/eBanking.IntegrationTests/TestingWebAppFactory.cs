using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using eBanking.Data;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eBanking.IntegrationTests
{
    public class TestingWebAppFactory<T> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var serviceProvider = new ServiceCollection()
                      .AddEntityFrameworkInMemoryDatabase()
                      .BuildServiceProvider();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryHomeControllerTest");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        try
                        {
                            appContext.Database.EnsureCreated();
                        }
                        catch (Exception ex)
                        {
                            //Log errors or do anything you think it's needed
                            Log.Information(ex, "CATHED ERROR: Database isn't created!");
                        }
                    }
                }
            });
        }
    }
}
