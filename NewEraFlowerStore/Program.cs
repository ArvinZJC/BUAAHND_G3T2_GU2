#region Using Directives
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NewEraFlowerStore.Data;
#endregion Using Directives

namespace NewEraFlowerStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            var services = host.Services.CreateScope().ServiceProvider;
            var environment = services.GetRequiredService<IHostingEnvironment>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            // try to initialised the database if the current hosting environment is development
            if (environment.IsDevelopment())
            {
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    DbInitialiser.InitialiseAsync(userManager, roleManager, context);
                    logger.LogInformation("System initialised database successfully.");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error! An error occurred initialising database.");
                } // end try...catch
            } // end if

            host.Run();
        } // end main

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    } // end class Program
} // end namespace NewEraFlowerStore