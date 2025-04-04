using Domain.directory;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.FluentApi.directory;

internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        //alternate key
        builder.HasAlternateKey(e => e.Username);
        builder.HasAlternateKey(e => e.Email);

        //foreign keys

        //many emplyee can have one job title on Job.Id and Employee.JobTitleId.
        //Not all require job title to be present, null job title
        //when employee is deleted, restrict deletion of job title
        builder
            .HasOne(e => e.JobTitle)
            .WithMany(j => j.Employees)
            .HasForeignKey(e => e.JobTitleId)
            .HasPrincipalKey( job => job.Id)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(e => e.Department)
            .WithMany(e => e.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .HasPrincipalKey( d=> d.Id)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);


        //self reference keys
        builder
            .HasOne(e => e.Manager)
            .WithMany(m => m.PrimaryStaff)
            .HasForeignKey(e => e.ManagerId)
            .HasPrincipalKey ( m => m.Id)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        //concurrency
        builder
            .Property(e => e.RowVersion).IsRowVersion();


    }
}
