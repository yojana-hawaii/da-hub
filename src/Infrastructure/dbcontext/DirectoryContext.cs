using Domain.directory;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.dbcontext;

public class DirectoryContext : DbContext
{
    public DirectoryContext(DbContextOptions<DirectoryContext> options) : base(options) { }


    public DbSet<Employee> Employees { get; set; }
    public DbSet<JobTitle> JobTitles { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Fax> Faxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //apply all fluent api configuration to entity using reflection
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
