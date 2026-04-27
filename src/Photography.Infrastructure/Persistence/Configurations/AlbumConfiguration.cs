using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Core.Albums;

namespace Photography.Infrastructure.Persistence.Configurations;

public sealed class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> b)
    {
        b.ToTable("albums");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).HasMaxLength(Album.MaxTitleLength).IsRequired();
        b.Property(x => x.Description).HasColumnType("text");
        b.Property(x => x.Client).HasMaxLength(Album.MaxClientLength);
        b.Property(x => x.Location).HasMaxLength(Album.MaxLocationLength);
        b.Property(x => x.EventDate);
        b.Property(x => x.ShowInPortfolio).IsRequired();
        b.Property(x => x.ShowInStories).IsRequired();
        b.Property(x => x.ShowInHome).IsRequired();
        b.Property(x => x.CategoryId).IsRequired();
        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.Property(x => x.UpdatedAtUtc);

        b.HasIndex(x => x.CategoryId);
        b.HasIndex(x => x.ShowInPortfolio);
        b.HasIndex(x => x.ShowInHome);

        b.HasMany(x => x.Images)
            .WithOne()
            .HasForeignKey(i => i.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(x => x.Images).Metadata.SetField("_images");
        b.Navigation(x => x.Images).UsePropertyAccessMode(PropertyAccessMode.Field);

        b.Ignore(x => x.DomainEvents);
    }
}
