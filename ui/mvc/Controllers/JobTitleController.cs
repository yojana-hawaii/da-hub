using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.directory;
using Infrastructure.dbcontext;

namespace mvc.Controllers;

public class JobTitleController : Controller
{
    private readonly DirectoryContext _context;

    public JobTitleController(DirectoryContext context)
    {
        _context = context;
    }

    // GET: JobTitle
    public async Task<IActionResult> Index()
    {
        return View(await _context.JobTitles.AsNoTracking().ToListAsync());
    }

    // GET: JobTitle/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var jobTitle = await _context.JobTitles
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (jobTitle == null)
        {
            return NotFound();
        }

        return View(jobTitle);
    }

    // GET: JobTitle/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: JobTitle/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("JobTitleName,JobTitleDescription")] JobTitle jobTitle)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("Cannot insert duplicate key row in object 'dbo.JobTitles' with unique index 'ix_jobTitle_name'"))
            {
                ModelState.AddModelError("JobTitleName", "Unable to save duplicate job title.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
            }
        }
        return View(jobTitle);
    }

    // GET: JobTitle/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var jobTitle = await _context.JobTitles.FindAsync(id);
        if (jobTitle == null)
        {
            return NotFound();
        }
        return View(jobTitle);
    }

    // POST: JobTitle/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("JobTitleName,JobTitleDescription")] JobTitle jobTitle)
    {
        if (id != jobTitle.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(jobTitle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobTitleExists(jobTitle.Id))
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
        return View(jobTitle);
    }

    // GET: JobTitle/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var jobTitle = await _context.JobTitles
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (jobTitle == null)
        {
            return NotFound();
        }

        return View(jobTitle);
    }

    // POST: JobTitle/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var jobTitle = await _context.JobTitles.FindAsync(id);
        if (jobTitle != null)
        {
            _context.JobTitles.Remove(jobTitle);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool JobTitleExists(int id)
    {
        return _context.JobTitles.Any(e => e.Id == id);
    }
}
