using Code_Judge.Application.Common.Interfaces;
using Code_Judge.Domain.Entities;
using Code_Judge.Infrastructure.ExecuteCode;
using Code_Judge.Infrastructure.Identity;
using Code_Judge.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Respawn;
using WebUi;

namespace Code_Judge.Application.IntegrationTests;

[SetUpFixture]
public partial class Testing
{
    private static WebApplicationFactory<Program> _factory = null!;
    private static IConfiguration _configuration = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Respawner _checkpoint = null!;
    private static string? _currentUserId;
    

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        _factory = new CustomWebApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var conStr = _configuration.GetConnectionString("DefaultConnection");
        var conn = new MySqlConnection(conStr);
        conn.Open();
        _checkpoint = Respawner.CreateAsync(conn, new RespawnerOptions
        {
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
            DbAdapter =  DbAdapter.MySql,
        }).GetAwaiter().GetResult();
        using var scope = _factory.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        initialiser.SeedAsync().GetAwaiter().GetResult();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }
    public static T GetService<T>()
    {
        
        
        using var scope = _scopeFactory.CreateScope();
        
        var service = scope.ServiceProvider.GetRequiredService<T>();

        return service;
    }
    public static async Task SendAsync(IBaseRequest request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }

    public static string? GetCurrentUserId()
    {
        return _currentUserId;
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>());
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { "Administrator" });
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var existingUser = await userManager.FindByNameAsync(userName);
         ApplicationUser user = null!;
         IdentityResult? result = null;
        if (existingUser == null)
        {
            user = new ApplicationUser { UserName = userName, Email = userName };
    
             result = await userManager.CreateAsync(user, password);
        }
        else
        {
            user = existingUser;
            
        }

      

        if (roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                if(await userManager.IsInRoleAsync(user, role))
                    continue;
                await roleManager.CreateAsync(new IdentityRole(role));
            }
            
            await userManager.AddToRolesAsync(user, roles);
        }
        

        if (result?.Succeeded ?? true)
        {
            _currentUserId = user.Id;

            return _currentUserId;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public static async Task ResetState()
    {
        try
        {
            
            var conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")!);
            await  conn.OpenAsync();
            await _checkpoint.ResetAsync(conn);
        }
        catch (Exception)
        {
            // ignored
        }

        _currentUserId = null;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }
}
