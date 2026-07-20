using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Worlds;

internal sealed class WorldConfiguration : IEntityTypeConfiguration<World>
{
    public void Configure(EntityTypeBuilder<World> builder)
    {
        builder.ToTable("worlds");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Width)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(w => w.Height)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Ignore(w => w.HasChanges);

        // Настройка CreatedAt
        builder.Property(w => w.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("NOW()");

        builder.HasMany(w => w.Scents)
                .WithOne()
                .HasForeignKey(s => s.WorldId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(w => w.Scents)
                .Metadata
                .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
