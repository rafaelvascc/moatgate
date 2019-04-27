using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;
using AutoMapper;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MoatGate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityServer4";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            var webhost = BuildWebHost(args);

            using (var serviceScope = webhost.Services.CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                var context = serviceScope.ServiceProvider.GetRequiredService<MoatGateIdentityDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<MoatGateIdentityUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<MoatGateIdentityRole>>();
                configurationContext.Database.Migrate();
                context.Database.Migrate();
                Seed(userManager, roleManager, context).Wait();
                Seed(configurationContext).Wait();
            }

            webhost.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSetting("https_port", "5100")
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .Build();

        public async static Task Seed(UserManager<MoatGateIdentityUser> userManager, RoleManager<MoatGateIdentityRole> roleManager, MoatGateIdentityDbContext context)
        {
            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new MoatGateIdentityRole() { Name = "IdentityAdmin" });
            }

            if (!context.Users.Any())
            {
                var user = new MoatGateIdentityUser
                {
                    UserName = "admin",
                    Email = "admin@moatgate.com",
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                await userManager.CreateAsync(user, "10qp!)QP");
                await userManager.AddToRoleAsync(user, "IdentityAdmin");
            }
        }

        public async static Task Seed(ConfigurationDbContext context)
        {
            if (!context.IdentityResources.Any())
            {
                context.IdentityResources.Add(Mapper.Map<IdentityServer4.EntityFramework.Entities.IdentityResource>(new IdentityResources.Address()));
                context.IdentityResources.Add(Mapper.Map<IdentityServer4.EntityFramework.Entities.IdentityResource>(new IdentityResources.Email()));
                context.IdentityResources.Add(Mapper.Map<IdentityServer4.EntityFramework.Entities.IdentityResource>(new IdentityResources.OpenId()));
                context.IdentityResources.Add(Mapper.Map<IdentityServer4.EntityFramework.Entities.IdentityResource>(new IdentityResources.Phone()));
                context.IdentityResources.Add(Mapper.Map<IdentityServer4.EntityFramework.Entities.IdentityResource>(new IdentityResources.Profile()));
                await context.SaveChangesAsync();
            }
        }
    }
}
