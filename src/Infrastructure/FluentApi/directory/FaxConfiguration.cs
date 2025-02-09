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
    }
}
