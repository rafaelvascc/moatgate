using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MoatGate.Models.AspNetIIdentityCore.EntityFramework;
using AutoMapper;
using MoatGate.Models.User;

namespace MoatGate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var identityServerConnectionString = Configuration.GetConnectionString("MoatGate.IdentityServer.ConnectionString");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<MoatGateIdentityDbContext>(options =>
            {
                options.UseSqlServer(identityServerConnectionString);
                options.EnableSensitiveDataLogging(true);
            });

            services.AddIdentity<MoatGateIdentityUser, MoatGateIdentityRole>()
                .AddUserManager<UserManager<MoatGateIdentityUser>>()
                .AddRoleManager<RoleManager<MoatGateIdentityRole>>()
                .AddEntityFrameworkStores<MoatGateIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });

            // configure identity server
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<MoatGateIdentityUser>()
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseSqlServer(identityServerConnectionString,
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                            });
                    };
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(identityServerConnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });

            services.AddMvc();

            Mapper.Initialize(c =>
            {
                c.CreateMap<IdentityServer4.EntityFramework.Entities.Client, IdentityServer4.EntityFramework.Entities.Client>()
                .ForMember(e => e.AllowedCorsOrigins, opt => opt.Ignore())
                .ForMember(e => e.AllowedGrantTypes, opt => opt.Ignore())
                .ForMember(e => e.AllowedScopes, opt => opt.Ignore())
                .ForMember(e => e.Claims, opt => opt.Ignore())
                .ForMember(e => e.ClientSecrets, opt => opt.Ignore())
                .ForMember(e => e.IdentityProviderRestrictions, opt => opt.Ignore())
                .ForMember(e => e.PostLogoutRedirectUris, opt => opt.Ignore())
                .ForMember(e => e.Properties, opt => opt.Ignore())
                .ForMember(e => e.RedirectUris, opt => opt.Ignore());

                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientCorsOrigin, IdentityServer4.EntityFramework.Entities.ClientCorsOrigin>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientGrantType, IdentityServer4.EntityFramework.Entities.ClientGrantType>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientScope, IdentityServer4.EntityFramework.Entities.ClientScope>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientClaim, IdentityServer4.EntityFramework.Entities.ClientClaim>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientSecret, IdentityServer4.EntityFramework.Entities.ClientSecret>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientIdPRestriction, IdentityServer4.EntityFramework.Entities.ClientIdPRestriction>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri, IdentityServer4.EntityFramework.Entities.ClientPostLogoutRedirectUri>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientProperty, IdentityServer4.EntityFramework.Entities.ClientProperty>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ClientRedirectUri, IdentityServer4.EntityFramework.Entities.ClientRedirectUri>().ForMember(e => e.Client, opt => opt.Ignore());

                c.CreateMap<IdentityServer4.EntityFramework.Entities.ApiResource, IdentityServer4.EntityFramework.Entities.ApiResource>()
                .ForMember(e => e.Scopes, opt => opt.Ignore())
                .ForMember(e => e.Secrets, opt => opt.Ignore())
                .ForMember(e => e.UserClaims, opt => opt.Ignore());

                c.CreateMap<IdentityServer4.EntityFramework.Entities.ApiScope, IdentityServer4.EntityFramework.Entities.ApiScope>()
                .ForMember(e => e.ApiResource, opt => opt.Ignore())
                .ForMember(e => e.UserClaims, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ApiSecret, IdentityServer4.EntityFramework.Entities.ApiSecret>().ForMember(e => e.ApiResource, opt => opt.Ignore());
                c.CreateMap<IdentityServer4.EntityFramework.Entities.ApiResourceClaim, IdentityServer4.EntityFramework.Entities.ApiResourceClaim>().ForMember(e => e.ApiResource, opt => opt.Ignore());

                c.CreateMap<IdentityServer4.EntityFramework.Entities.ApiScopeClaim, IdentityServer4.EntityFramework.Entities.ApiScopeClaim>()
                .ForMember(e => e.ApiScope, opt => opt.Ignore());

                c.CreateMap<IdentityServer4.EntityFramework.Entities.IdentityResource, IdentityServer4.EntityFramework.Entities.IdentityResource>()
                .ForMember(e => e.UserClaims, opt => opt.Ignore());

                c.CreateMap<IdentityServer4.EntityFramework.Entities.IdentityClaim, IdentityServer4.EntityFramework.Entities.IdentityClaim>()
                .ForMember(e => e.IdentityResource, opt => opt.Ignore());

                c.CreateMap<MoatGateIdentityUser, MoatGateIdentityUserEditViewModel>()
                .ForSourceMember(s => s.NormalizedUserName, opt => opt.Ignore())
                .ForSourceMember(s => s.NormalizedEmail, opt => opt.Ignore())
                .ForSourceMember(s => s.EmailConfirmed, opt => opt.Ignore())
                .ForSourceMember(s => s.PasswordHash, opt => opt.Ignore())
                .ForSourceMember(s => s.SecurityStamp, opt => opt.Ignore())
                .ForSourceMember(s => s.ConcurrencyStamp, opt => opt.Ignore())
                .ForSourceMember(s => s.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForSourceMember(s => s.LockoutEnd, opt => opt.Ignore())
                .ForSourceMember(s => s.LockoutEnabled, opt => opt.Ignore())
                .ForSourceMember(s => s.AccessFailedCount, opt => opt.Ignore())
                .IncludeBase<MoatGateIdentityUser, MoatGateIdentityUserViewModel>().ReverseMap()
                .ForSourceMember(s => s.SendResetPasswordEmail, opt => opt.Ignore())
                .IncludeBase<MoatGateIdentityUserViewModel, MoatGateIdentityUser>();
                
                c.CreateMap<MoatGateIdentityUserCreateViewModel, MoatGateIdentityUser>()
                .ForSourceMember(s => s.ConfirmPassword, opt => opt.Ignore())
                .ForSourceMember(s => s.Password, opt => opt.Ignore())
                .IncludeBase<MoatGateIdentityUserViewModel, MoatGateIdentityUser>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
