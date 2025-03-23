using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

internal class FaxConfiguration : IEntityTypeConfiguration<Fax>
{
    public void Configure(EntityTypeBuilder<Fax> builder)
    {
        builder
            .HasIndex(f => f.FaxNumber)
            .IsUnique();

        //one location can have many faxes with relation between location.id and fax.locationid
        builder
            .HasOne(f => f.Location)
            .WithMany(f => f.Faxes)
            .HasForeignKey(f => f.LocationId)
            .HasPrincipalKey( loc => loc.Id)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(f => f.Department)
            .WithMany(f => f.Faxes)
            .HasForeignKey(f => f.DepartmentId)
            .HasPrincipalKey(dept => dept.Id)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
