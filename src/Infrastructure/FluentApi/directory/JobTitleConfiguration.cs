using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

internal class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
{
    public void Configure(EntityTypeBuilder<JobTitle> builder)
    {
        builder
            .HasIndex(h => h.JobTitleName)
            .IsUnique()
            .HasDatabaseName("ix_jobTitle_name");
    }
}
