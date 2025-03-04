using Domain.directory;
using Infrastructure.dbcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc.ViewModel;

namespace mvc.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly DirectoryContext _context;

        public EmployeeController(DirectoryContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var directoryContext = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Manager)
                .Include(e => e.EmployeeLocations).ThenInclude(el => el.Location)
                .AsNoTracking();
            return View(await directoryContext.ToListAsync());
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Manager)
                .Include(e => e.EmployeeLocations).ThenInclude(el => el.Location)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            Employee employee = new();
            PopulateLocationCheckboxList(employee);
            PopulateDropdownLists(); 
            return View(employee);
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Email,FirstName,LastName,Extension,PhoneNumber,Keyword,NickName,EmployeeNumber,PhotoPath,JobTitleId,DepartmentId,ManagerId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdownLists(employee);
            return View(employee);
        }



        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            PopulateDropdownLists(employee);
            return View(employee);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Username,Email,AccountCreated,FirstName,LastName,Extension,PhoneNumber,Keyword,HireDate,NickName,EmployeeNumber,PhotoPath,JobTitleId,DepartmentId,ManagerId")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            PopulateDropdownLists(employee);
            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Manager)
                .Include(e => e.EmployeeLocations).ThenInclude(el => el.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        /// <summary>
        /// good for create and delete. Edit needs a little more
        /// allOptions has all locations, IsSelected is true only if current employee has locationId in their intersection table
        /// </summary>
        /// <param name="employee"></param>
        private void PopulateLocationCheckboxList(Employee employee)
        {
            var allOptions = _context.Locations;
            var currentSelectedIds = new HashSet<int>(employee.EmployeeLocations.Select(el => el.LocationId));
            var checkBoxes = new List<CheckOptionVM>();

            foreach(var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    Id = option.Id,
                    DisplayText = option.Summary,
                    IsSelected = currentSelectedIds.Contains(option.Id)
                });
            }
            ViewData["LocationOptions"] = checkBoxes;
        }

        private void PopulateDropdownLists(Employee? employee = null)
        {

            ViewData["DepartmentId"] = Departmentlist(employee?.DepartmentId);
            ViewData["JobTitleId"] = JobTitleList(employee?.JobTitleId);
            ViewData["ManagerId"] = ManagerList(employee?.ManagerId);
        }

        private SelectList ManagerList(int? managerId)
        {
            return new SelectList(_context.Employees.OrderBy(m => m.Summary), "Id", "Summary", managerId);
        }

        private SelectList JobTitleList(int? jobTitleId)
        {
            return new SelectList(_context.JobTitles.OrderBy(j => j.JobTitleName), "Id", "JobTitleName", jobTitleId);
        }

        private SelectList Departmentlist(int? departmentId)
        {
            return new SelectList(_context.Departments.OrderBy(d => d.DepartmentName), "Id", "DepartmentName", departmentId);
        }
    }
}
