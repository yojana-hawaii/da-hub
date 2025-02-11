using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

internal class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder
            .HasIndex(i => new { i.LocationName, i.SubLocation})
            .IsUnique()
            .HasDatabaseName("ix_location_name");
        builder
            .HasIndex(i => i.SubLocation)
            .HasDatabaseName("ix_location_sublocation");
    }
}
