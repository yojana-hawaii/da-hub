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
            .IsUnique()
            .HasDatabaseName("ix_fax_number");
        builder
            .HasOne(f => f.Location)
            .WithMany(f => f.Faxes)
            .HasForeignKey(f => f.LocationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(f => f.Department)
            .WithMany(f => f.Faxes)
            .HasForeignKey(f => f.DepartmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
