using System.Reflection;
using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using Code_Judge.Infrastructure.ExecuteCode;
using Code_Judge.Infrastructure.Files;
using Code_Judge.Infrastructure.Identity;
using Code_Judge.Infrastructure.Persistence;
using Code_Judge.Infrastructure.Persistence.Interceptors;
using Code_Judge.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
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
                options.UseMySQL(configuration.GetConnectionString("DefaultConnection")??throw new InvalidOperationException(),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();
        var executeCodeStrategyTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type =>
                typeof(IExecuteCodeStrategy).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        foreach (var executeCodeStrategyType in executeCodeStrategyTypes)
        {
            services.AddTransient( executeCodeStrategyType);
            
        }
        services.AddSingleton<IExecuteCodeStrategyFactory, ExecuteCodeStrategyFactory>();
        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        services.AddAuthentication()
            .AddIdentityServerJwt();

        services.AddAuthorization(options =>
            options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        return services;
    }
}
