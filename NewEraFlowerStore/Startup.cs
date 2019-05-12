#region Using Directives
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Licensing;

using NewEraFlowerStore.Data;
using NewEraFlowerStore.Services;
#endregion Using Directives

namespace NewEraFlowerStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        } // end constructor Startup

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            }); // configure an IIS Server option for hosting the application in-process
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true; // this lambda determines whether user consent for non-essential cookies is needed for a given request
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DataAccessFromMySql")));
            services.AddOptions();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>(); // ".AddDefaultUI(UIFramework.Bootstrap4)" is not required because the Identity scaffolder was used to add Identity files to the project to customise Identity
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); // configure this Lockout setting and leave the others to default
                
                // configure the following Password settings and leave the others to default
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                // configure User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                options.User.RequireUniqueEmail = true;
            }); // configure options for the Identity area
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
            });
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeAreaFolder("Admin", "/Bouquets", "RequireAdministratorRole");
                    options.Conventions.AuthorizeAreaFolder("Admin", "/Colours", "RequireAdministratorRole");
                    options.Conventions.AuthorizeAreaFolder("Admin", "/Flowers", "RequireAdministratorRole");
                    options.Conventions.AuthorizeAreaFolder("Admin", "/Occasions", "RequireAdministratorRole");
                    options.Conventions.AuthorizeAreaFolder("Admin", "/Orders", "RequireAdministratorRole");
                    options.Conventions.AuthorizeAreaFolder("Admin", "/OtherAdministrators", "RequireAdministratorRole");
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage/AddressBooks", "RequireCustomerRole");
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage/Orders", "RequireCustomerRole");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
            });

            services.AddHttpsRedirection(options => options.HttpsPort = 5001);
            services.AddSingleton<ICaptchaManager, CaptchaManager>(); // add the service for managing the captcha
            services.AddSingleton<IEmailSender, EmailSender>(); // add the service for sending emails containing verification links
        } // end method ConfigureServices

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment environment)
        {
            SyncfusionLicenseProvider.RegisterLicense("OTk3MjlAMzEzNzJlMzEyZTMwbUFjSmgzU2MwU2VGWUg3N01zTDRZcmhIeDd0aFM1T0JSREFOdTZtZ21Zdz0="); // register the license key for Syncfusion Essential JS 2 (Version 17.1.0.44)

            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;

                    if (context.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                    {
                        context.Response.ContentType = "text/html";
                        await context.Response.SendFileAsync($@"{environment.WebRootPath}/errors/500.html");
                    }
                })); // use the customised error page for the error code 500
                app.UseStatusCodePagesWithReExecute("/errors/{0}");
                app.UseHsts(); // keep the default HSTS max-age value (30 days)
            } // end if...else

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes => routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}"));
        } // end method Configure
    } // end class Startup
} // end namespace NewEraFlowerStore