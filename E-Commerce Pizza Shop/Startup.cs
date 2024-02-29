using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PizzaApp.Models;
using PizzaApp.Service;
using System;
using System.Globalization;

namespace PizzaApp
{
    public class Startup
    {
        private IWebHostEnvironment _env;
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.local.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            var mongoSettings = Configuration.GetSection(nameof(MongoDbSettings));
            var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            services.AddSingleton<MongoDbSettings>(settings);
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
            })
                    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(settings.ConnectionString, settings.DatabaseName)
                    .AddSignInManager()
                    .AddDefaultTokenProviders();
            services.AddSingleton<TicketService>();
            services.AddSingleton<FoodService>();
            services.AddSingleton<OrderService>();
            var builder = services.AddRazorPages();
#if DEBUG
            if (_env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif

            services.AddSession();
            services.AddApplicationInsightsTelemetry();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            var cultureInfo = new CultureInfo("pl-PL");
            cultureInfo.NumberFormat.CurrencySymbol = "zł"; 
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
