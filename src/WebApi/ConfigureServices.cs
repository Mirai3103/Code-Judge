using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using Code_Judge.Infrastructure.Persistence;
using Code_Judge.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using ZymLabs.NSwag.FluentValidation;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddControllersWithViews();

        services.AddRazorPages(
           );
        

        services.AddScoped<FluentValidationSchemaProcessor>(provider =>
        {
            var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });

        services.AddIdentityServer();
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
        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddOpenApiDocument((configure, serviceProvider) =>
        {
            var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<FluentValidationSchemaProcessor>();

            // Add the fluent validations schema processor
            configure.SchemaProcessors.Add(fluentValidationSchemaProcessor);

            configure.Title = "Code_Judge API";
            configure.AddSecurity("OAuth2", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Flow = NSwag.OpenApiOAuth2Flow.Implicit,
                AuthorizationUrl = "https://localhost:5251/connect/authorize",
                TokenUrl = "https://localhost:5251/connect/token",
                
                Name = "Authorization",
                Scopes = new Dictionary<string, string>
                {
                    {  "api.read", "Read Access to API" },
                    {  "api.write", "Write Access to API" }
                },
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        Scopes = new Dictionary<string, string>
                        {
                            {  "api.read", "Read Access to API" },
                            {  "api.write", "Write Access to API" }
                        },
                        TokenUrl = "https://localhost:5251/connect/token",
                        AuthorizationUrl = "https://localhost:5251/connect/authorize",

                    },
                }
                
            });
            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("OAuth2"));
        });

        return services;
    }
}
