using Infrastructure.dbcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc.CustomController;

namespace mvc.Controllers;

public class LookupController : CognizantController
{
    private readonly DirectoryContext _context;

    public LookupController(DirectoryContext context)
    {
        _context = context;
    }

    public IActionResult Index(string Tab = "Department-Tab")
    {
        // parameter viewdata and pass it back to view
        ViewData["Tab"] = Tab; 
        return View();
    }
    public PartialViewResult Department()
    {
        ViewData["DepartmentDropdown"] = new SelectList(_context.Departments.OrderBy(d => d.DepartmentName), "Id", "DepartmentName");
        return PartialView("_Department");
    }
    public PartialViewResult JobTitle()
    {
        ViewData["JobTitleDropdown"] = new SelectList(_context.JobTitles.OrderBy(j => j.JobTitleName), "Id", "JobTitleName");
        return PartialView("_JobTItle");
    }
    public PartialViewResult Location()
    {
        ViewData["LocationDropdown"] = new SelectList(_context.Locations.OrderBy(l => l.LocationName), "Id", "LocationName");
        return PartialView("_Location");
    }
}
