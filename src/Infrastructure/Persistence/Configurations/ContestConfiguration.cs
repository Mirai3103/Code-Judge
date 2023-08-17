using Code_Judge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Code_Judge.Infrastructure.Persistence.Configurations;


public class ContestConfiguration: IEntityTypeConfiguration<Contest>
{
    public void Configure(EntityTypeBuilder<Contest> builder)
    {
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode();
        builder.Property(t => t.EndTime).IsRequired();
        builder.Property(t => t.StartTime).IsRequired();
        builder.Property(t => t.Description).IsRequired(false)
            .HasDefaultValue(null).HasColumnType("text").IsUnicode();
        builder.HasMany<Problem>(t => t.Problems).WithOne(t => t.Contest)
            .HasForeignKey(t => t.ContestId).OnDelete(DeleteBehavior.Cascade);
    }
}