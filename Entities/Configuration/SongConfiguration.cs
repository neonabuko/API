using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SongManager.Entities;

namespace Entities.Configuration;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.Property(s => s.Name)
        .IsRequired()
        .HasMaxLength(255);
    }
}