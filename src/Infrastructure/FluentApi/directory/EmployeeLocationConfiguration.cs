using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

public class EmployeeLocationConfiguration : IEntityTypeConfiguration<EmployeeLocation>
{
    public void Configure(EntityTypeBuilder<EmployeeLocation> builder)
    {
        builder.HasKey(e => new { e.EmployeeId, e.LocationId });

        builder
            .HasOne(el => el.Location)
            .WithMany(el => el.EmployeeLocations)
            .HasForeignKey(el => el.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(el => el.Employee)
            .WithMany(el => el.EmployeeLocations)
            .HasForeignKey(el => el.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
