using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.FluentApi.directory;
internal class DepartmentDocumentConfiguration : IEntityTypeConfiguration<DepartmentDocument>
{
    public void Configure(EntityTypeBuilder<DepartmentDocument> builder)
    {
        builder
            .HasOne(dd => dd.Department)
            .WithMany(dd => dd.DepartmentDocuments)
            .HasForeignKey(dd => dd.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
