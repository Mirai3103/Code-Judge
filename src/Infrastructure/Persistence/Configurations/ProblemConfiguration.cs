using Code_Judge.Domain.Entities;
using Code_Judge.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Code_Judge.Infrastructure.Persistence.Configurations;

public class ProblemConfiguration: IEntityTypeConfiguration<Problem>
{
    public void Configure(EntityTypeBuilder<Problem> builder)
    {
        builder.Property(t => t.Description)
            .IsRequired()
            .HasColumnType("text")
            .IsUnicode();
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode();
        builder.Property(t => t.Points)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(t=>t.MemoryLimit)
            .IsRequired()
            .HasColumnType("float");
        builder.Property(t => t.TimeLimit)
            .IsRequired();
        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(t=>t.Hint)
            .IsRequired(false)
            .HasDefaultValue(null)
            .HasMaxLength(1000)
            .IsUnicode();
        builder.Property(t=>t.DifficultyLevel)
            .IsRequired()
            .HasDefaultValue(DifficultyLevel.Easy);
        builder.Property(t => t.TemplateCode)
            .IsRequired()
            .HasColumnType("text")
            .IsUnicode(false);
        builder.HasMany<TestCase>(t => t.TestCases)
            .WithOne(t=>t.Problem)
            .HasForeignKey(t => t.ProblemId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany<Submission>(t => t.Submissions)
            .WithOne(t => t.Problem)
            .HasForeignKey(t => t.ProblemId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Editorial>(t => t.Editorial)
            .WithOne(t=>t.Problem)
            .HasForeignKey<Editorial>(t => t.ProblemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}