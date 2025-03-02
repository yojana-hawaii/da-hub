using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.directory;
using Infrastructure.dbcontext;

namespace mvc.Controllers;

public class LocationController : Controller
{
    private readonly DirectoryContext _context;

    public LocationController(DirectoryContext context)
    {
        _context = context;
    }

    // GET: Location
    public async Task<IActionResult> Index()
    {
        return View(await _context.Locations.AsNoTracking().OrderBy(l => l.LocationName).ThenBy(s => s.SubLocation).ToListAsync());
    }

    // GET: Location/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var location = await _context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (location == null)
        {
            return NotFound();
        }

        return View(location);
    }



    // GET: Location/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Location/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("LocationName,SubLocation")] Location location)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException dex)
        {
            string err = dex.GetBaseException().Message;
            if (err.Contains("unique") && err.Contains("ix_location"))
            {
                ModelState.AddModelError("LocationName", "Unable to save duplicate location / sub-location pair.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
            }
        }
        catch (Exception)
        {
            throw;
        }
        return View(location);
    }




    // GET: Location/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }
        return View(location);
    }

    // POST: Location/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id)
    {
        var locToUpdate = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
        if (locToUpdate == null)
        {
            return NotFound();
        }

        if (await TryUpdateModelAsync<Location>(locToUpdate, "", l => l.LocationName, l => l.SubLocation))
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(locToUpdate.Id))
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
                if (err.Contains("unique") && err.Contains("ix_location"))
                {
                    ModelState.AddModelError("LocationName", "Unable to save duplicate location / sub-location pair.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
                }
            }
        }
        return View(locToUpdate);
    }




    // GET: Location/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var location = await _context.Locations
            .FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        return View(location);
    }

    // POST: Location/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        try
        {
            if (location != null)
            {
                _context.Locations.Remove(location);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                ModelState.AddModelError("", "Unable to delete record. Location already used");
            }
            else
            {
                ModelState.AddModelError("", "Unable to delete record. Cant think of a reason this could happen");
            }
        }
        return View(location);
    }




    private bool LocationExists(int id)
    {
        return _context.Locations.Any(e => e.Id == id);
    }
}
