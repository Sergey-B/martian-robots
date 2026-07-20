using Domain.Scents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Scents;

internal sealed class ScentConfiguration : IEntityTypeConfiguration<Scent>
{
    public void Configure(EntityTypeBuilder<Scent> builder)
    {
        builder.ToTable("scents");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.WorldId)
               .IsRequired();

        builder.Property(s => s.X)
               .IsRequired();

        builder.Property(s => s.Y)
               .IsRequired();

        builder.Property(s => s.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("NOW()");

        builder.HasIndex(s => s.WorldId);
    }
}
