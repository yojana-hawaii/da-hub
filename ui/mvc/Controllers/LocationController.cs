using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.directory;
using Infrastructure.dbcontext;
using mvc.CustomController;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers;

//[Authorize]
public class LocationController : CustomLookupsController
{
    private readonly DirectoryContext _context;

    public LocationController(DirectoryContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        // ReturnToAction(string ActionName, string ControllerName, string Fragment)
        return RedirectToAction("Index", "Lookup", new { Tab = ViewData["QueryStringValueOrNavTabName"]?.ToString() });
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
        _context.Add(location);
        return await TryCatchSaveChanges(location);
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
            return await TryCatchSaveChanges(locToUpdate);
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
        if (location != null)
        {
            _context.Locations.Remove(location);
            return await TryCatchSaveChanges(location);
        }
        return View(location);
    }




    private bool LocationExists(int id)
    {
        return _context.Locations.Any(e => e.Id == id);
    }

    private async Task<IActionResult> TryCatchSaveChanges(Location location)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _context.SaveChangesAsync();
                // ReturnToAction(string ActionName, string ControllerName, string Fragment)
                return RedirectToAction("Index", "Lookup", new {Tab = ViewData["QueryStringValueOrNavTabName"]?.ToString() });
            }
        }
        catch (DbUpdateConcurrencyException ex)
        {
            string errMessage = ex.GetBaseException().Message;
            if (!LocationExists(location.Id))
            {
                return NotFound();
            }
            else
            {
                ModelState.AddModelError("", errMessage);
            }
        }
        catch (DbUpdateException ex)
        {
            string errMessage = ex.GetBaseException().Message;
            if (errMessage.Contains("unique") && errMessage.Contains("IX_Locations_LocationName"))
            {
                ModelState.AddModelError("LocationName", "Unable to save duplicate location.");
            }
            else if (errMessage.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                ModelState.AddModelError("", "Unable to delete record. Location already used");
            }
            else
            {
                ModelState.AddModelError("", errMessage);
            }
        }
        catch (Exception ex)
        {
            string errMessage = ex.GetBaseException().Message;
            ModelState.AddModelError("", errMessage);
        }
        return View(location);
    }

}
