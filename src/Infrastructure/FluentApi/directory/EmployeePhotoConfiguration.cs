using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

internal class EmployeePhotoConfiguration : IEntityTypeConfiguration<EmployeePhoto>
{
    public void Configure(EntityTypeBuilder<EmployeePhoto> builder)
    {
        builder
            .HasOne(ep => ep.Employee)
            .WithOne(ep => ep.EmployeePhoto)
            .HasForeignKey<EmployeePhoto>(ep => ep.EmployeeId);
    }
}
