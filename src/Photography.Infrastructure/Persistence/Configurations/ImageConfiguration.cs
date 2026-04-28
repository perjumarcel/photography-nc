using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photography.Core.Albums;

namespace Photography.Infrastructure.Persistence.Configurations;

public sealed class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> b)
    {
        b.ToTable("images");
        b.HasKey(x => x.Id);
        // Domain-generated Guid; see AlbumConfiguration for the full explanation.
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.AlbumId).IsRequired();
        b.Property(x => x.OriginalName).HasMaxLength(Image.MaxOriginalNameLength).IsRequired();
        b.Property(x => x.StorageKey).HasMaxLength(Image.MaxStorageKeyLength).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(Image.MaxContentTypeLength).IsRequired();
        b.Property(x => x.Checksum).HasMaxLength(Image.MaxChecksumLength);
        b.Property(x => x.SizeBytes).IsRequired();
        b.Property(x => x.Width).IsRequired();
        b.Property(x => x.Height).IsRequired();
        b.Property(x => x.Orientation).HasConversion<int>().IsRequired();
        b.Property(x => x.ImageType).HasConversion<int>().IsRequired();
        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.Property(x => x.UpdatedAtUtc);

        b.HasIndex(x => x.AlbumId);
        b.HasIndex(x => x.StorageKey).IsUnique();
        b.HasIndex(x => new { x.AlbumId, x.ImageType });
    }
}
