using Code_Judge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Code_Judge.Infrastructure.Persistence.Configurations;

public class SubmissionConfiguration: IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.Property((t=>t.Code))    .IsRequired()
            .HasColumnType("text")
            .IsUnicode(false);
        builder.Property(t => t.Language).IsRequired();
        builder.Property((t=>t.Error))   .IsRequired(false)
            .HasMaxLength(255)
            .IsUnicode(false);
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.Memory).IsRequired().HasColumnType("float");
        builder.Property(t => t.RunTime).IsRequired();
        builder.Property(t => t.Note).IsRequired(false).HasMaxLength(200);
        
    }
}