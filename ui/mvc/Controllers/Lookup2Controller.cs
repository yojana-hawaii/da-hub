using Infrastructure.dbcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvc.Controllers;

public class Lookup2Controller : Controller
{
    private readonly DirectoryContext _context;

    public Lookup2Controller(DirectoryContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewData["JobTitleDropdown"] = new SelectList(_context.JobTitles.OrderBy(j => j.JobTitleName), "Id", "JobTitleName");
        ViewData["DepartmentDropdown"] = new SelectList(_context.Departments.OrderBy(d => d.DepartmentName), "Id", "DepartmentName");
        ViewData["LocationDropdown"] = new SelectList(_context.Locations.OrderBy(l => l.LocationName), "Id", "LocationName");
        return View();
    }
}
