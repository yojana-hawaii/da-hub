using Domain.directory;
using Domain.extension;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.dbcontext;

public class DirectoryContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    public string? LoggedInUser { get; private set; }

    public DirectoryContext(DbContextOptions<DirectoryContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        if (_httpContextAccessor.HttpContext != null)
        {
            LoggedInUser = _httpContextAccessor.HttpContext.User.Identity?.Name ?? "Unknown";
        }
        else
        {
            LoggedInUser = "seed-data-constructor-overload";

        }
    }
    public DirectoryContext(DbContextOptions<DirectoryContext> options) : base(options)
    {
        _httpContextAccessor = null;
        LoggedInUser = "seed-data-base-consstructor";
    }


    public DbSet<Employee> Employees { get; set; }
    public DbSet<JobTitle> JobTitles { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Fax> Faxes { get; set; }
    public DbSet<EmployeeLocation> EmployeeeLocations { get; set; }

    public DbSet<UploadedFile> UploadedFiles { get; set; }
    //no DbSet for UploadedFileContent > One to one > always created and always belong to UploadedFile
    public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //apply all fluent api configuration to entity using reflection
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void OnBeforeSaving()
    {
        var entities = ChangeTracker.Entries();
        foreach (var entity in entities)
        {
            if (entity.Entity is IAuditable trackable)
            {
                var now = DateTime.Now;
                switch (entity.State)
                {
                    case EntityState.Modified:
                        trackable.ModifiedBy = LoggedInUser;
                        trackable.ModifiedDate = now;
                        break;
                    case EntityState.Added:
                        trackable.ModifiedBy = LoggedInUser;
                        trackable.ModifiedDate = now;
                        trackable.CreatedBy = LoggedInUser;
                        trackable.CreatedDate = now;
                        break;
                }
            }
        }
    }
}
