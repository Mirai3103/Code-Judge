using Code_Judge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Code_Judge.Infrastructure.Persistence.Configurations;

internal class EditorialConfiguration: IEntityTypeConfiguration<Editorial>
{
    public void Configure(EntityTypeBuilder<Editorial> builder)
    {
        builder.Property(t => t.Content).IsRequired()
            .HasColumnType("text")
            .IsUnicode();
    }
}