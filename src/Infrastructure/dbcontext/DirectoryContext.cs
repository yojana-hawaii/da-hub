using Domain.directory;
using Domain.directoryViewModel;
using Domain.extension;
using Infrastructure.FluentApi.directory;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
        LoggedInUser = "seed-data-base-constructor";
    }

    #region DbSet Table and View 
    public DbSet<Employee> Employees { get; set; }
    public DbSet<JobTitle> JobTitles { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Fax> Faxes { get; set; }
    public DbSet<EmployeeLocation> EmployeeeLocations { get; set; }

    public DbSet<UploadedFile> UploadedFiles { get; set; }
    //no DbSet for UploadedFileContent > One to one > always created and always belong to UploadedFile
    public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
    public DbSet<DepartmentDocument> DepartmentDocuments { get; set; }

    public DbSet<EmployeePhoto> EmployeePhotos { get; set; }
    public DbSet<EmployeeThumbnail> EmployeeThumbnails { get; set; }

    // create data base view
    public DbSet<ManagerSummaryVM> ManagerSummaries { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //apply all fluent api configuration to entity using reflection
        //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        #region Manual Mapping Table Configuration
        modelBuilder.Entity<Employee>();
        modelBuilder.Entity<JobTitle>();
        modelBuilder.Entity<Department>();
        modelBuilder.Entity<Location>();
        modelBuilder.Entity<Fax>();
        modelBuilder.Entity<EmployeeLocation>();
        modelBuilder.Entity<UploadedFile>();
        modelBuilder.Entity<EmployeeDocument>();
        modelBuilder.Entity<DepartmentDocument>();
        modelBuilder.Entity<EmployeePhoto>();
        modelBuilder.Entity<EmployeeThumbnail>();

        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new JobTitleConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        modelBuilder.ApplyConfiguration(new FaxConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeLocationConfiguration());
        modelBuilder.ApplyConfiguration(new UploadedFileConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeDocumentConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentDocumentConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeePhotoConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeThumbnailConfiguration());

        #endregion

        //manual mapping view
        modelBuilder
            .Entity<ManagerSummaryVM>()
            .ToView(nameof(ManagerSummaries));
        modelBuilder.ApplyConfiguration(new ManagerSummaryVMConfiguration());

        modelBuilder.HasDefaultSchema("dbo");
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
