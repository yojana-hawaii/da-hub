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
    /// <param name="UseMigrations">User Migration or EnsureCreated</param>
    /// <param name="SeedSampleData">Add Sample data</param>
    public static void Initialize(IServiceProvider serviceProvider,
        bool DeleteDatabase = false, bool UseMigrations = true, bool SeedSampleData = true)
    {
        using DirectoryContext _context = new(serviceProvider.GetRequiredService<DbContextOptions<DirectoryContext>>());

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
            Random rnd = new();

            try
            {
                if (!_context.Locations.Any())
                {
                    SeedLocation(_context);
                }

                if (!_context.JobTitles.Any())
                {
                    SeedJobTitle(_context);
                }

                if (!_context.Departments.Any())
                {
                    SeedDepartment(_context);
                }

                if (!_context.Faxes.Any())
                {
                    SeedFax(_context, rnd);
                }

                if (!_context.Employees.Any())
                {
                    SeedEmployee(_context, rnd);
                }

                if (!_context.EmployeeeLocations.Any())
                {
                    SeedEmployeeLocation(_context, rnd);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
        #endregion
    }

    private static void SeedEmployeeLocation(DirectoryContext context, Random rnd)
    {
        //Ids could be deleted over time. Need to select what is available
        int[] empIds = context.Employees.Select(s => s.Id).ToArray();
        var empCount = empIds.Length;

        int[] locIds = context.Locations.Select(s => s.Id).ToArray();
        var locCount = locIds.Length;

        foreach( var id in empIds)
        {
            int howManyLocations = rnd.Next(1, 5);
            for(int i =1; i <= howManyLocations; i++)
            {
                int locId = rnd.Next(1, locCount + 1);
                EmployeeLocation empLoc = new EmployeeLocation()
                {
                    EmployeeId = id,
                    LocationId = locId
                };
                try
                {
                    context.EmployeeeLocations.Add(empLoc);
                }
                catch (Exception ex)
                {
                    context.EmployeeeLocations.Remove(empLoc);
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
            }
        }

        try
        {
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.GetBaseException().Message);
        }
    }

    private static void SeedEmployee(DirectoryContext context, Random rnd)
    {
        var firstnames = new string[] { "Trent", "Virgil", "Abrille", "Diamond", "Aurelia", "Harvey", "Dara", "Della", "Everest", "Juniper", "Kai", "Kiara", "Napheesa", "Lorraine", "Rafael" };
        var lastnames = new string[] { "Gerrard", "Van Dijk", "Quansah", "Salah", "Diaz", "Nunez", "Gakpo", "Konate", "Becker", "Anthony-Towns", "Grugier-Hill", "Bynum", "Hockenson", "Mundt", "Tyson" };

        var totalEmp = 30;

        var firstCount = firstnames.Length;
        var lastCount = lastnames.Length;
        var deptCount = context.Departments.Count();

        var jobCount = context.JobTitles.Count();

        for (var i = 0; i <= totalEmp; i++)
        {
            var first = firstnames[rnd.Next(0, firstCount)];
            var last = lastnames[rnd.Next(0, lastCount)];
            var extension = rnd.Next(500, 599);

            var emp = new Employee
            {
                FirstName = first,
                LastName = last,
                Username = $"{first}.{last}",
                Email = $"{first}.{last}@email.com",
                Extension = $"{extension}",
                PhoneNumber = $"8002158{extension}",
                JobTitleId = rnd.Next(1, jobCount + 1),
                DepartmentId = rnd.Next(1, deptCount + 1)
            };

            try
            {
                context.Employees.Add(emp);
            }
            catch (Exception ex)
            {
                context.Employees.Remove(emp);
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
        try
        {
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.GetBaseException().Message);
        }

        foreach (var emp in context.Employees)
        {
            emp.ManagerId = rnd.Next(1, 5);
        }
        try
        {
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.GetBaseException().Message);
        }
    }

    private static void SeedFax(DirectoryContext context, Random random)
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

        try
        {
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.GetBaseException().Message);
        }
    }

    private static void SeedJobTitle(DirectoryContext context)
    {
        var jobTitles = new string[] { "big-boss", "baby-boss", "wannabe-boss", "boss-lady", "not-boss" };
        foreach (var job in jobTitles)
        {
            var j = new JobTitle { JobTitleName = job };
            context.JobTitles.Add(j);
        }
        try
        {
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.GetBaseException().Message);
        }
    }

    private static void SeedLocation(DirectoryContext context)
    {
        var buildings = new string[] { "Building-A", "Building-B", "Building-C", "Building-D" };

        var sub1 = new string[] { "Floor 1", "Floor 2", "Floor 3" };
        var sub2 = new string[] { "Exec", "Accounting", "Marketing" };
        var sub3 = new string[] { "Room 1", "Room 2", "Room 3" };
        var sub = new string[][] { sub1, sub2, sub3 };

        var buildingCount = buildings.Length;
        var subCount = sub.Length;
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

    private static void SeedDepartment(DirectoryContext context)
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
        try
        {
            context.Departments.AddRange(departments);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.GetBaseException().Message);
        }
    }
}
