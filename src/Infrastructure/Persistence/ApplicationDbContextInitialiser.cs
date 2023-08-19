using System.Text.Json;
using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using Code_Judge.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.EntityFrameworkCore.Extensions;

namespace Code_Judge.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsMySql())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole("Administrator");

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
            }
        }

        // Default data
        // Seed, if necessary
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items =
                {
                    new TodoItem { Title = "Make a todo list 📃" },
                    new TodoItem { Title = "Check off the first item ✅" },
                    new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
                    new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
                }
            });

            await _context.SaveChangesAsync();
        }

        if (!_context.Problems.Any())
        {
            //start transaction
            var transaction = await _context.Database.BeginTransactionAsync();
            var problem = new Problem
            {
                IsPublic = true,
                Name = "Bài toán Two Sum",
                Description = "### Bài toán Two Sum\n\nCho một mảng các số nguyên và một giá trị mục tiêu (target), hãy tìm hai số trong mảng mà tổng của chúng bằng giá trị mục tiêu.\n\n**Đầu vào:**\n- n: số lượng phần tử trong mảng\n- n phần tử tiếp theo: các phần tử của mảng\n- target: giá trị mục tiêu\n\n**Đầu ra:**\n- Hai chỉ số của mảng mà tổng của hai phần tử tại các chỉ số đó bằng giá trị mục tiêu, cách nhau bằng một dấu cách.",
                Points = 1,
                DifficultyLevel = DifficultyLevel.Easy,
                Hint = "Hãy thử sử dụng một bảng băm để lưu trữ các giá trị bù của mỗi phần tử.",
                TimeLimit = 1,
                MemoryLimit = 1,
                Slug = "two-sum",
            };
            await _context.Problems.AddAsync(problem);
            await _context.SaveChangesAsync();
            var testcaseJsonPath = "E:\\projects\\Code-Judge\\src\\Infrastructure\\Persistence\\Migrations\\testcase.json";
            var testcaseJson = await File.ReadAllTextAsync(testcaseJsonPath);
            var testcases = JsonSerializer.Deserialize<List<TestCase>>(testcaseJson)??throw new Exception("Cannot deserialize testcases");
            foreach (var testcase in testcases)
            {
                testcase.ProblemId = problem.Id;
                await _context.TestCases.AddAsync(testcase);
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            
        }
    }
}

