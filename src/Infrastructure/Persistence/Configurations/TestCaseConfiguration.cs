using Code_Judge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Code_Judge.Infrastructure.Persistence.Configurations;

public class TestCaseConfiguration: IEntityTypeConfiguration<TestCase>
{
    public void Configure(EntityTypeBuilder<TestCase> builder)
    {
        builder.Property(t => t.Input)
            .IsRequired()
            .HasColumnType("text")
            .IsUnicode(false);
        builder.Property(t => t.Output)
            .IsRequired()
            .IsUnicode(false);
        builder.Property(t => t.IsHidden)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(t => t.Id).ValueGeneratedOnAdd()
            .IsRequired();
        builder.HasKey(t => t.Id);
    }
}