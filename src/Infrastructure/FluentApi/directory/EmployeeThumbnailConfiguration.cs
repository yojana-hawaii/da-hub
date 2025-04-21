using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

internal class EmployeeThumbnailConfiguration : IEntityTypeConfiguration<EmployeeThumbnail>
{
    public void Configure(EntityTypeBuilder<EmployeeThumbnail> builder)
    {
        builder
            .HasOne(et => et.Employee)
            .WithOne(et => et.EmployeeThumbnail)
            .HasForeignKey<EmployeeThumbnail>(et => et.EmployeeId);
    }
}
