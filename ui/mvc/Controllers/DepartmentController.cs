using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.directory;
using Infrastructure.dbcontext;
using mvc.CustomController;

namespace mvc.Controllers;

public class DepartmentController : ReturnUrlController
{
    private readonly DirectoryContext _context;

    public DepartmentController(DirectoryContext context)
    {
        _context = context;
    }

    // GET: Department
    public async Task<IActionResult> Index()
    {
        return View(await _context.Departments.AsNoTracking().ToListAsync());
    }

    // GET: Department/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (department == null)
        {
            return NotFound();
        }

        return View(department);
    }

    // GET: Department/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Department/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("DepartmentName")] Department department)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException dex)
        {
            string err = dex.GetBaseException().Message;
            if (err.Contains("unique") && err.Contains("ix_department_name"))
            {
                ModelState.AddModelError("DepartmentName", "Unable to save duplicate department name.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
            }
        }
        return View(department);
    }

    // GET: Department/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }
        return View(department);
    }

    // POST: Department/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id)
    {
        var deptToUpdate = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        if (deptToUpdate == null)
        {
            return NotFound();
        }

        if (await TryUpdateModelAsync<Department>(deptToUpdate, "", d => d.DepartmentName))
        {
            try
            {
                _context.Update(deptToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(deptToUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException dex)
            {
                string err = dex.GetBaseException().Message;
                if (err.Contains("unique") && err.Contains("ix_department_name"))
                {
                    ModelState.AddModelError("DepartmentName", "Unable to save duplicate department name.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
                }
            }
        }
        return View(deptToUpdate);
    }

    // GET: Department/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (department == null)
        {
            return NotFound();
        }

        return View(department);
    }

    // POST: Department/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id);
        try
        {
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                ModelState.AddModelError("", "Unable to delete record. Department already used");
            }
            else
            {
                ModelState.AddModelError("", "Unable to delete record. Cant think of a reason this could happen");
            }
        }
        return View(department);
    }
    private bool DepartmentExists(int id)
    {
        return _context.Departments.Any(e => e.Id == id);
    }
}
