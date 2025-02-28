using Domain.directory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Infrastructure.dbcontext;

public static class DirectoryInitializer
{
    /// <summary>
    /// prepare database and seed data
    /// </summary>
    /// <param name="serviceProvider">DI Container</param>
    /// <param name="DeleteDatabase">Delete database and start from scratch</param>
    /// <param name="UserMigrations">User Migration or EnsureCreated</param>
    /// <param name="SeedSampleData">Add Sample data</param>
    public static void Initialize(IServiceProvider serviceProvider,
        bool DeleteDatabase = false, bool UseMigrations = true, bool SeedSampleData = true)
    {
        using (var _context = new DirectoryContext(serviceProvider.GetRequiredService<DbContextOptions<DirectoryContext>>()))
        {
            #region Prepare the database depedning on options
            try
            {
                //using migration
                if (UseMigrations)
                {
                    if (DeleteDatabase)
                    {
                        _context.Database.EnsureDeleted(); //delete exisiting version
                    }
                    _context.Database.Migrate(); // apply all migrationa
                }
                else //no migration.Delete everything and recreate
                {
                    if (DeleteDatabase)
                    {
                        _context.Database.EnsureDeleted();
                        _context.Database.EnsureCreated();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
            #endregion

            #region Add data that is applicable to sample or production data
            #endregion

            #region Seed sample data
            if (SeedSampleData)
            {
                Random rnd = new Random();

                var user = "manual-seed";
                var now = DateTime.Now;
                try
                {
                    if (!_context.Locations.Any())
                    {
                        SeedLocation(_context, user, now);
                    }

                    if (!_context.JobTitles.Any())
                    {
                        SeedJobTitle(_context, user, now);
                    }

                    if (!_context.Departments.Any())
                    {
                        SeedDepartment(_context, user, now);
                    }

                    if (!_context.Faxes.Any())
                    {
                        SeedFax(_context, user, now, rnd);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
            }
            #endregion
        }
    }

    private static void SeedFax(DirectoryContext context, string user, DateTime now, Random random)
    {
        var locCount = context.Locations.Count();
        var deptCount = context.Departments.Count();
        var namePrefix = "fax";


        for (var i = 1; i <= 20; i++)
        {
            var deptId = random.Next(0, deptCount + 1);
            var locId = random.Next(0, locCount + 1);
            var original = "800999" + random.Next(1000, 9999);
            var destination = "800777" + random.Next(1000, 9999);
            var forward = random.Next() % 2 == 0;

            var fax = new Fax
            {
                FaxName = namePrefix + i,
                FaxNumber = original,
                IsForwarded = forward,
                ForwardTo = forward ? destination : null,
                DepartmentId = deptId == 0 ? null : deptId,
                LocationId = locId == 0 ? null : locId
            };

            try
            {
                context.Faxes.Add(fax);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }

        context.SaveChanges();
    }

    private static void SeedJobTitle(DirectoryContext context, string user, DateTime now)
    {
        var jobTitles = new string[] { "ceo", "payroll", "manager", "chef" };
        foreach (var job in jobTitles)
        {
            var j = new JobTitle { JobTitleName = job };
            context.JobTitles.Add(j);
        }
        ;
        context.SaveChanges();
    }

    private static void SeedLocation(DirectoryContext context, string user, DateTime now)
    {
        var buildings = new string[] { "Building-A", "Building-B", "Building-C", "Building-D" };

        var sub1 = new string[] { "Floor 1", "Floor 2", "Floor 3" };
        var sub2 = new string[] { "Exec", "Accounting", "Marketing" };
        var sub3 = new string[] { "Room 1", "Room 2", "Room 3" };
        var sub = new string[][] { sub1, sub2, sub3 };

        var buildingCount = buildings.Count();
        var subCount = sub.Count();
        var max = buildingCount > subCount ? buildingCount : subCount;

        try
        {
            for (int i = 0; i < max; i++)
            {

                if (i >= subCount)
                {
                    var loc = new Location
                    {
                        LocationName = buildings[i]
                    };
                    context.Locations.Add(loc);
                }
                else
                {
                    foreach (var s in sub[i])
                    {
                        var loc = new Location
                        {
                            LocationName = buildings[i],
                            SubLocation = s
                        };
                        context.Locations.Add(loc);
                    }
                }
            }
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.GetBaseException().Message);
        }
    }

    private static void SeedDepartment(DirectoryContext context, string user, DateTime now)
    {
        var departments = new[]
        {
            new Department{ DepartmentName = "accounting" },
            new Department{ DepartmentName = "exec" },
            new Department{ DepartmentName = "IT" },
            new Department{ DepartmentName = "engineering" },
            new Department{ DepartmentName = "marketing" },
            new Department{ DepartmentName = "delivery" }
        };
        context.Departments.AddRange(departments);
        context.SaveChanges();
    }


}
