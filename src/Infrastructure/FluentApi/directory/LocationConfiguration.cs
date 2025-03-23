using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

internal class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder
           .Property(c => c.ComputedSubLocationForUniqueness)
           .HasComputedColumnSql("isnull(SubLocation, 'NULL-MARKER')");
        builder
            .HasIndex(i => new { i.LocationName, i.ComputedSubLocationForUniqueness})
            .IsUnique();
      
    }
}
