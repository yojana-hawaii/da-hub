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
        return View(await _context.Locations.AsNoTracking().ToListAsync());
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
            if (dex.GetBaseException().Message.Contains("Cannot insert duplicate key row in object 'dbo.Locations' with unique index"))
            {
                ModelState.AddModelError("LocationName", "Unable to save duplicate location / sub-location pair.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
            }
        }
        catch(Exception ex)
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
    public async Task<IActionResult> Edit(int id, [Bind("LocationName,SubLocation")] Location location)
    {
        if (id != location.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(location);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(location.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(location);
    }




    // GET: Location/Delete/5
    public async Task<IActionResult> Delete(int? id)
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

    // POST: Location/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location != null)
        {
            _context.Locations.Remove(location);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }




    private bool LocationExists(int id)
    {
        return _context.Locations.Any(e => e.Id == id);
    }
}
