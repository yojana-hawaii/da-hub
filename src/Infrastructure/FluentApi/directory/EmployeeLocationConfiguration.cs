using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

public class EmployeeLocationConfiguration : IEntityTypeConfiguration<EmployeeLocation>
{
    public void Configure(EntityTypeBuilder<EmployeeLocation> builder)
    {
        builder.HasKey(e => new { e.EmployeeId, e.LocationId });


        //if location deleted, keep employee record of location - but why
        builder
            .HasOne(el => el.Location)
            .WithMany(el => el.EmployeeLocations)
            .HasForeignKey(el => el.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        //if employee delete, cascade delete the record
        builder
            .HasOne(el => el.Employee)
            .WithMany(el => el.EmployeeLocations)
            .HasForeignKey(el => el.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
