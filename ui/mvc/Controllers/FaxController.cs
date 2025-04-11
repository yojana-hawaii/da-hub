using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.directory;
using Infrastructure.dbcontext;
using Microsoft.EntityFrameworkCore.Storage;
using mvc.CustomController;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers;

public class FaxController : ReturnUrlController
{
    private readonly DirectoryContext _context;

    public FaxController(DirectoryContext context)
    {
        _context = context;
    }

    // GET: Fax
    public async Task<IActionResult> Index()
    {
        var faxes = _context.Faxes
            .Include(f => f.Department)
            .Include(f => f.Location)
            .AsNoTracking();

        ViewData["TotalObjects"] = await faxes.CountAsync();

        return View(await faxes.ToListAsync());
    }

    // GET: Fax/Details/5
    [Authorize]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fax = await _context.Faxes
            .Include(f => f.Department)
            .Include(f => f.Location)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (fax == null)
        {
            return NotFound();
        }

        return View(fax);
    }

    // GET: Fax/Create
    [Authorize]
    public IActionResult Create()
    {
        PopulateDropdownList();
        return View();
    }

    // POST: Fax/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create([Bind("FaxName,FaxNumber,IsForwarded,ForwardTo,LocationId,DepartmentId")] Fax fax)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(fax);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (RetryLimitExceededException) // begin tranaction & rollback automatic ef core
        {
            ModelState.AddModelError("", "Unable to save after multiple attemps. Call Microsoft.");
        }
        catch (DbUpdateException dex)
        {
            string err = dex.GetBaseException().Message;
            if (err.Contains("unique") && err.Contains("ix_fax_number"))
            {
                ModelState.AddModelError("FaxNumber", "Unable to save duplicate fax number.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
            }
        }

        PopulateDropdownList(fax);
        return View(fax);
    }

    // GET: Fax/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fax = await _context.Faxes.FindAsync(id);
        if (fax == null)
        {
            return NotFound();
        }
        PopulateDropdownList(fax);
        return View(fax);
    }

    // POST: Fax/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var faxToUpdate = await _context.Faxes.FirstOrDefaultAsync(f => f.Id == id);

        if (faxToUpdate == null)
        {
            return NotFound();
        }

        //removing default bind in parameter like in create method. Hidden properties like createdDate will be deleted.
        if (await TryUpdateModelAsync<Fax>(faxToUpdate, "",
            f => f.FaxName, f => f.FaxNumber, f => f.IsForwarded, f => f.ForwardTo, f => f.LocationId, f => f.DepartmentId
            ))
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FaxExists(faxToUpdate.Id))
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
                if (err.Contains("unique") && err.Contains("ix_fax_number"))
                {
                    ModelState.AddModelError("FaxNumber", "Unable to save duplicate fax number.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
                }
            }
        }
        PopulateDropdownList(faxToUpdate);
        return View(faxToUpdate);
    }

    // GET: Fax/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var fax = await _context.Faxes
            .Include(f => f.Department)
            .Include(f => f.Location)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (fax == null)
        {
            return NotFound();
        }

        return View(fax);
    }

    // POST: Fax/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var fax = await _context.Faxes
            .Include(f => f.Department)
            .Include(f => f.Location)
            .FirstOrDefaultAsync(m => m.Id == id);

        try
        {
            if (fax != null)
            {
                _context.Faxes.Remove(fax);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                ModelState.AddModelError("", "Unable to delete record. Fax already used");
            }
            else
            {
                ModelState.AddModelError("", "Unable to delete record. Cant think of a reason this could happen");
            }
        }
        return View(fax);
    }

    private bool FaxExists(int id)
    {
        return _context.Faxes.Any(e => e.Id == id);
    }
    
    private SelectList LocationList(int? selectedLocationId)
    {
        var query = from d in _context.Locations
                    orderby d.LocationName, d.SubLocation
                    select d;
        var ret =  new SelectList(query, "Id", "Summary", selectedLocationId);
        return ret;
    }
    private SelectList DepartmentList(int? selectedDepartmentId)
    {
        var ret =  new SelectList(_context.Departments.OrderBy(d => d.DepartmentName),"Id", "DepartmentName", selectedDepartmentId);
        return ret;
    }
    private void PopulateDropdownList(Fax? fax = null)
    {
        ViewData["LocationId"] = LocationList(fax?.LocationId);
        ViewData["DepartmentId"] = DepartmentList(fax?.DepartmentId);
    }

    
}
