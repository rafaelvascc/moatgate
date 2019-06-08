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
using IdentityServer4.EntityFramework.Entities;
using System.Linq;
using MoatGate.Services;
using Swashbuckle.AspNetCore.Swagger;
using MoatGate.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MoatGate
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configLoader = new EnvironmentConfigLoader(_env, Configuration);

            var identityServerConnectionString = configLoader.GetDbConnectionString();

            if (string.IsNullOrEmpty(identityServerConnectionString))
                throw new ApplicationException("Could't find Moatgate Connection String from environment variable MOATGATE_DB_CONNECTION_STRING or configuration MoatGateIdentityServerConnectionString on the appSettings.json file");

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

            var (useRedisTicketStore, redisServer) = configLoader.GetRedisOptions();

            if (useRedisTicketStore && string.IsNullOrWhiteSpace(redisServer))
                throw new ApplicationException("Could't find Redis serer configuration from environment variable MOATGATE_REDIS_CONNECTION_STRING or configuration RedisConnectionString on the appSettings.json file");

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
                options.SessionStore = useRedisTicketStore ? new RedisTicketStore(redisServer) : (ITicketStore)new InMemoryTicketStore();
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

            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeFolder("/Profile", "IsAuthenticated");
                options.Conventions.AuthorizePage("/Account/ChangePassword", "IsAuthenticated");
                options.Conventions.AuthorizeFolder("/Client", "IsIdentityAdmin");
                options.Conventions.AuthorizeFolder("/Resources", "IsIdentityAdmin");
                options.Conventions.AuthorizeFolder("/Roles", "IsIdentityAdmin");
                options.Conventions.AuthorizeFolder("/Users", "IsIdentityAdmin");

                options.Conventions.AddPageRoute("/Account/Login", "");
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAuthenticated", policy => policy.RequireAuthenticatedUser());
                options.AddPolicy("IsIdentityAdmin", policy => policy.RequireRole("IdentityAdmin"));
                options.AddPolicy("IsAuthorizedUserApiClient", policy =>
                {
                    policy.RequireScope("moatgate_api_users");
                });
            });

            //TODO: use IdentityServer4.Hosting.LocalAccessTokenValidation when identity server 3.0 ships
            services.AddAuthentication()
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = configLoader.GetApplicationUrl();
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "moatgate_api";
                    options.ApiSecret = "moatgate_api_secret";
                });

            var (useSendgrid, sendgridOptions) = configLoader.GetSendGridOptions();
            var (useTwilio, twilioOptions) = configLoader.GetTwillioOptions();

            if (useSendgrid)
            {
                services.AddSingleton<IEmailSender, SendGridEmailSender>();
                services.AddSingleton(sendgridOptions);
            }
            else
            {
                services.AddSingleton<IEmailSender, DummyMessagingService>();
            }

            if (useTwilio)
            {
                services.AddSingleton<ISmsSender, TwilioSmsSender>();
                services.AddSingleton(twilioOptions);
            }
            else
            {
                services.AddSingleton<ISmsSender, DummyMessagingService>();
            }

            Mapper.Initialize(c =>
            {
                c.CreateMap<Client, Client>()
                .ForMember(e => e.AllowedCorsOrigins, opt => opt.Ignore())
                .ForMember(e => e.AllowedGrantTypes, opt => opt.Ignore())
                .ForMember(e => e.AllowedScopes, opt => opt.Ignore())
                .ForMember(e => e.Claims, opt => opt.Ignore())
                .ForMember(e => e.ClientSecrets, opt => opt.Ignore())
                .ForMember(e => e.IdentityProviderRestrictions, opt => opt.Ignore())
                .ForMember(e => e.PostLogoutRedirectUris, opt => opt.Ignore())
                .ForMember(e => e.Properties, opt => opt.Ignore())
                .ForMember(e => e.RedirectUris, opt => opt.Ignore());

                c.CreateMap<ClientCorsOrigin, ClientCorsOrigin>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<ClientGrantType, ClientGrantType>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<ClientScope, ClientScope>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<ClientClaim, ClientClaim>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<ClientSecret, ClientSecret>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<ClientIdPRestriction, ClientIdPRestriction>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUri>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<ClientProperty, ClientProperty>().ForMember(e => e.Client, opt => opt.Ignore());
                c.CreateMap<ClientRedirectUri, ClientRedirectUri>().ForMember(e => e.Client, opt => opt.Ignore());

                c.CreateMap<ApiResource, ApiResource>()
                .ForMember(e => e.Scopes, opt => opt.Ignore())
                .ForMember(e => e.Secrets, opt => opt.Ignore())
                .ForMember(e => e.UserClaims, opt => opt.Ignore());

                c.CreateMap<ApiScope, ApiScope>()
                .ForMember(e => e.ApiResource, opt => opt.Ignore())
                .ForMember(e => e.UserClaims, opt => opt.Ignore());
                c.CreateMap<ApiSecret, ApiSecret>().ForMember(e => e.ApiResource, opt => opt.Ignore());
                c.CreateMap<ApiResourceClaim, ApiResourceClaim>().ForMember(e => e.ApiResource, opt => opt.Ignore());

                c.CreateMap<ApiScopeClaim, ApiScopeClaim>()
                .ForMember(e => e.ApiScope, opt => opt.Ignore());

                c.CreateMap<IdentityResource, IdentityResource>()
                .ForMember(e => e.UserClaims, opt => opt.Ignore());

                c.CreateMap<IdentityClaim, IdentityClaim>()
                .ForMember(e => e.IdentityResource, opt => opt.Ignore());

                c.CreateMap<UserCreateViewModel, MoatGateIdentityUser>()
                .ForSourceMember(s => s.ConfirmPassword, opt => opt.DoNotValidate())
                .ForSourceMember(s => s.Password, opt => opt.DoNotValidate());

                c.CreateMap<UserCreateApiViewModel, MoatGateIdentityUser>()
                .ForSourceMember(s => s.ConfirmPassword, opt => opt.DoNotValidate())
                .ForSourceMember(s => s.Password, opt => opt.DoNotValidate());

                c.CreateMap<IdentityServer4.Models.IdentityResources.Address, IdentityResource>()
                .ForMember(r => r.Id, opt => opt.Ignore())
                .ForMember(r => r.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(cl => new IdentityClaim { Type = cl }).ToList()));
                c.CreateMap<IdentityServer4.Models.IdentityResources.Email, IdentityResource>()
                .ForMember(r => r.Id, opt => opt.Ignore())
                .ForMember(r => r.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(cl => new IdentityClaim { Type = cl }).ToList()));
                c.CreateMap<IdentityServer4.Models.IdentityResources.OpenId, IdentityResource>()
                .ForMember(r => r.Id, opt => opt.Ignore())
                .ForMember(r => r.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(cl => new IdentityClaim { Type = cl }).ToList()));
                c.CreateMap<IdentityServer4.Models.IdentityResources.Phone, IdentityResource>()
                .ForMember(r => r.Id, opt => opt.Ignore())
                .ForMember(r => r.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(cl => new IdentityClaim { Type = cl }).ToList()));
                c.CreateMap<IdentityServer4.Models.IdentityResources.Profile, IdentityResource>()
                .ForMember(r => r.Id, opt => opt.Ignore())
                .ForMember(r => r.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(cl => new IdentityClaim { Type = cl }).ToList()));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Moat Gate Public API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new ApiKeyScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = "header",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            //if (!_env.IsDevelopment())
            //{
            //    services.AddHttpsRedirection(options =>
            //    {
            //        options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            //        options.HttpsPort = 443;
            //    });
            //}
            //else
            //{
            //    services.AddHttpsRedirection(options =>
            //    {
            //        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            //        options.HttpsPort = 5001;
            //    });
            //}

            //services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(1);
            //});
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
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseIdentityServer();

            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Moat Gate Public API");
            });

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
