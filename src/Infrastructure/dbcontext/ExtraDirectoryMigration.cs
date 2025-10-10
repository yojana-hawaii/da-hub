using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.dbcontext;

public static class ExtraDirectoryMigration
{
    public static void CreateViewWithRawSqlScript(IServiceProvider serviceProvider)
    {
		// round about way to get db context coz this is static class and cannot inject instance of dbContext
		using DirectoryContext _context = new DirectoryContext(serviceProvider.GetRequiredService<DbContextOptions<DirectoryContext>>());

		string drop = @"drop view if exists dbo.ManagerSummaries;";
        _context.Database.ExecuteSqlRaw(drop);

        string view = @"
				create view ManagerSummaries
				as
					select 
						isnull(mngr.Id, 0) Id, 
						isnull(mngr.FirstName, 'No Manager') FirstName, 
						isnull(mngr.LastName,'') LastName, 
						count(*) NumberOfStaff,
						max(cast(staff.accountCreated as date)) LatestHireDate,
						min(cast(staff.accountCreated as date)) OldestHireDate
					from dbo.Employees as staff
						left join dbo.Employees as mngr on staff.ManagerId = mngr.Id
					group by mngr.Id, mngr.FirstName, mngr.LastName
            ";

		_context.Database.ExecuteSqlRaw(view);

    }
}
