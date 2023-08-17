using Code_Judge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Code_Judge.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    DbSet<Contest> Contests { get; }
    DbSet<Editorial> Editorials { get; }
    DbSet<Problem> Problems { get; }
    DbSet<Submission> Submissions { get; }
    DbSet<TestCase> TestCases { get; }
    DbSet<ApplicationUser> ApplicationUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
