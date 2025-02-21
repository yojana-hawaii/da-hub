﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Domain.directory;
using Infrastructure.dbcontext;

namespace mvc.Controllers;

public class FaxController : Controller
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
        return View(await faxes.ToListAsync());
    }

    // GET: Fax/Details/5
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
    public IActionResult Create()
    {
        ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName");
        LocationDropdownList();
        return View();
    }

    // POST: Fax/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
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
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("Cannot insert duplicate key row in object 'dbo.Faxes' with unique index 'ix_fax_number'"))
            {
                ModelState.AddModelError("FaxNumber", "Unable to save duplicate fax number.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
            }
        }

        ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", fax.DepartmentId);
        LocationDropdownList(fax);
        return View(fax);
    }

    // GET: Fax/Edit/5
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
        ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", fax.DepartmentId);
        LocationDropdownList(fax);
        return View(fax);
    }

    // POST: Fax/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
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
                if (dex.GetBaseException().Message.Contains("Cannot insert duplicate key row in object 'dbo.Faxes' with unique index 'ix_fax_number'"))
                {
                    ModelState.AddModelError("FaxNumber", "Unable to save duplicate fax number.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
                }
            }
        }
        ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", faxToUpdate.DepartmentId);
        LocationDropdownList(faxToUpdate);
        return View(faxToUpdate);
    }

    // GET: Fax/Delete/5
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var fax = await _context.Faxes.FindAsync(id);
        if (fax != null)
        {
            _context.Faxes.Remove(fax);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool FaxExists(int id)
    {
        return _context.Faxes.Any(e => e.Id == id);
    }

    private void LocationDropdownList(Fax? fax = null)
    {
        var query = from d in _context.Locations
                    orderby d.LocationName, d.SubLocation
                    select d;
        //ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "LocationName", fax.LocationId);

        ViewData["LocationId"] = new SelectList(query, "Id", "Summary", fax?.LocationId);
    }

}
