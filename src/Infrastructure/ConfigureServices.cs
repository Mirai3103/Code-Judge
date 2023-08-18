using System.Reflection;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using Code_Judge.Infrastructure.ExecuteCode;
using Code_Judge.Infrastructure.Files;
using Code_Judge.Infrastructure.Identity;
using Code_Judge.Infrastructure.Persistence;
using Code_Judge.Infrastructure.Persistence.Interceptors;
using Code_Judge.Infrastructure.Services;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("Code_JudgeDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(
                    configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException(),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();
        var executeCodeStrategyTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type =>
                typeof(IExecuteCodeStrategy).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        foreach (var executeCodeStrategyType in executeCodeStrategyTypes)
        {
            services.AddTransient(executeCodeStrategyType);
        }

        services.AddSingleton<IExecuteCodeStrategyFactory, ExecuteCodeStrategyFactory>();
        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
            .AddInMemoryIdentityResources(new IdentityResource[]
            {
                new IdentityResources.OpenId(), new IdentityResources.Profile(),
            })
            .AddInMemoryApiResources(new ApiResource[] { new ApiResource("api", "Code_Judge API")
            {
                Enabled = true,
                Scopes = new List<string> { "api.read", "api.write" },
            } })
            .AddInMemoryApiScopes(new ApiScope[] { new ApiScope("api.read"), new ApiScope("api.write"), })
            .AddInMemoryClients(new Client[]
            {
                new()
                {
                    ClientId = "swagger",
                    ClientSecrets = { new Secret("a".Sha256()) },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api.read",
                        "api.write",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api"
                    },
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    RedirectUris = {"https://localhost:5251/api/oauth2-redirect.html"},
                    PostLogoutRedirectUris ={"https://localhost:5251/api/oauth2-redirect.html"},
                    AllowedCorsOrigins = {"https://localhost:5251"},
                },
            })
            .AddProfileService<ProfileService>()
            .AddDeveloperSigningCredential()
            ;

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority ="https://localhost:5251";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidAudiences = new List<string>() { "api", "api.read", "api.write" };
            }); 
   

        services.AddAuthorization(options =>
            options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        return services;
    }
}