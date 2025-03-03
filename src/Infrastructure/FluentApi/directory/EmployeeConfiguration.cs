using Domain.directory;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.FluentApi.directory;

internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        //alternate key
        builder.HasAlternateKey(e => e.Username).HasName("ix_employee_username");
        builder.HasAlternateKey(e => e.Email).HasName("ix_employee_email");

        //foreign keys
        builder
            .HasOne(e => e.JobTitle)
            .WithMany(j => j.Employees)
            .HasForeignKey(e => e.JobTitleId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(e => e.Department)
            .WithMany(e => e.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);


        //self reference keys
        builder
            .HasOne(e => e.Manager)
            .WithMany(m => m.PrimaryStaff)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(e => e.ManagerId)
            .IsRequired(false);

    }
}
