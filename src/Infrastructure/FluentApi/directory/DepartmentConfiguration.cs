using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

internal class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder
            .HasIndex(c => new { c.DepartmentName })
            .IsUnique()
            .HasDatabaseName("ix_department_name");
    }
}
