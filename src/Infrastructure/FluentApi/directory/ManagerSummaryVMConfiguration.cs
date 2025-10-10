using Domain.directoryViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;

internal class ManagerSummaryVMConfiguration : IEntityTypeConfiguration<ManagerSummaryVM>
{
    public void Configure(EntityTypeBuilder<ManagerSummaryVM> builder)
    {
        builder
           .HasKey( x=> x.Id );
    }
}
