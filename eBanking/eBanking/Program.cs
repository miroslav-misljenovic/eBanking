using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using eBanking.Data;
using eBanking.BusinessModels;
using Serilog;

namespace eBanking
{
    public class Program
    {
        public static void Main(string[] args)
        {            
             Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        
            try
            {
                Log.Information("Starting up");
                var host = CreateHostBuilder(args).Build();
                SetupData(host);
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            /* 
            var host = CreateHostBuilder(args).Build();
            SetupData(host);
            host.Run();
            */
        }
        private static void SetupData(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    CreateDefaultUsers(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        private static void CreateDefaultUsers(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Administrator";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Client").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Client";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Add administrator
            var prodAdministrator = new ApplicationUser()
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };
            if (userManager.FindByEmailAsync(prodAdministrator.Email).Result == null)
            {
                IdentityResult result = userManager.CreateAsync(prodAdministrator, "Admin123$").Result;
                if (result.Succeeded)
                {
                    var res = userManager.AddToRoleAsync(prodAdministrator, "Administrator").Result;

                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
