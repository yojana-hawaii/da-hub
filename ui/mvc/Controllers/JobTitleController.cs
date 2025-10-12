using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.directory;
using Infrastructure.dbcontext;
using mvc.CustomController;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace mvc.Controllers;

//[Authorize]
public class JobTitleController : CustomLookupsController
{
    private readonly DirectoryContext _context;

    public JobTitleController(DirectoryContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        // ReturnToAction(string ActionName, string ControllerName, string Fragment)
        return RedirectToAction("Index", "Lookup", new { Tab = ViewData["QueryStringValueOrNavTabName"]?.ToString() });
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
                // ReturnToAction(string ActionName, string ControllerName, string Fragment)
                return RedirectToAction("Index", "Lookup", new { Tab = ViewData["QueryStringValueOrNavTabName"]?.ToString() });
            }

        }
        catch (DbUpdateException dex)
        {
            string err = dex.GetBaseException().Message;
            if (err.Contains("unique") && err.Contains("ix_jobTitle_name"))
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
    public async Task<IActionResult> Edit(int id)
    {
        var jobToUpdate = await _context.JobTitles.FirstOrDefaultAsync(j => j.Id == id);
        if (jobToUpdate == null)
        {
            return NotFound();
        }

        if (await TryUpdateModelAsync<JobTitle>(jobToUpdate, "", j => j.JobTitleName, j => j.JobTitleDescription))
        {
            try
            {
                await _context.SaveChangesAsync();
                // ReturnToAction(string ActionName, string ControllerName, string Fragment)
                return RedirectToAction("Index", "Lookup", new { Tab = ViewData["QueryStringValueOrNavTabName"]?.ToString() });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobTitleExists(jobToUpdate.Id))
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
                if (err.Contains("unique") && err.Contains("ix_jobTitle_name"))
                {
                    ModelState.AddModelError("JobTitleName", "Unable to save duplicate job title.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save. Some problem I did not thing about.");
                }
            }
        }
        return View(jobToUpdate);
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
        var jobTitle = await _context.JobTitles.FirstOrDefaultAsync(j => j.Id == id);

        try
        {
        if (jobTitle != null)
        {
            _context.JobTitles.Remove(jobTitle);
        }

        await _context.SaveChangesAsync();
        // ReturnToAction(string ActionName, string ControllerName, string Fragment)
        return RedirectToAction("Index", "Lookup", new { Tab = ViewData["QueryStringValueOrNavTabName"]?.ToString() });

        }
        catch (DbUpdateException dex)
        {
            if (dex.GetBaseException().Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                ModelState.AddModelError("", "Unable to delete record. Job Title already used");
            }
            else
            {
                ModelState.AddModelError("", "Unable to delete record. Cant think of a reason this could happen");
            }
        }
        return View(jobTitle);
    }

    [HttpPost]
    public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
    {
        // no error handling and no duplicate check
        // try catch
        // how many records incoming, how many success import, identify reason for failure > show result in view
        ExcelPackage excel;
        using (var memoryStream = new MemoryStream())
        {
            await theExcel.CopyToAsync(memoryStream);
            excel = new ExcelPackage(memoryStream);
        }
        var worksheet = excel.Workbook.Worksheets[0];
        var start = worksheet.Dimension.Start;
        var end = worksheet.Dimension.End;

        List<JobTitle> jobTitles = new List<JobTitle>();

        for(int row = start.Row; row < end.Row; row++)
        {
            JobTitle jobTitle = new JobTitle
            {
                JobTitleName = worksheet.Cells[row, 1].Text
            };
            jobTitles.Add(jobTitle);
        }
        _context.JobTitles.AddRange(jobTitles);
        _context.SaveChanges();

        return Redirect(ViewData["returnUrl"].ToString());
    }
    private bool JobTitleExists(int id)
    {
        return _context.JobTitles.Any(e => e.Id == id);
    }
}
