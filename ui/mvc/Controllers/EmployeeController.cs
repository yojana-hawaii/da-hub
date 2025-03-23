using Domain.directory;
using Infrastructure.dbcontext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
            var employees = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Manager)
                .Include(e => e.EmployeeLocations).ThenInclude(el => el.Location)
                .AsNoTracking();
            return View(await employees.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Username,Email,FirstName,LastName,Extension,PhoneNumber," +
                            "NickName,EmployeeNumber,PhotoPath,JobTitleId,DepartmentId,ManagerId")] Employee employee, string[] selectedOptions)
        {
            try
            {
                if (selectedOptions != null)
                {
                    foreach (var location in selectedOptions)
                    {
                        var locationToAdd = new EmployeeLocation { LocationId = int.Parse(location), EmployeeId = employee.Id };
                        employee.EmployeeLocations.Add(locationToAdd);
                    }
                }
                if (ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save after multiple attempts.");
            }
            catch (DbUpdateException ex)
            {
                string err = ex.GetBaseException().Message;
                if (err.Contains("unique") && err.Contains("ix_employee_username"))
                {
                    ModelState.AddModelError("Username", "Unable to save duplicate username.");
                }
                else if (err.Contains("unique") && err.Contains("ix_employee_email"))
                {
                    ModelState.AddModelError("Username", "Unable to save duplicate email.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save for some reason");
                }
            }

            PopulateLocationCheckboxList(employee); // display selected checkbox even if there is validation error.
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
        public async Task<IActionResult> Edit(int id)
        {
            var employeeToUpdate = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employeeToUpdate == null)
            {
                return NotFound();
            }


            if (await TryUpdateModelAsync<Employee>(employeeToUpdate, "",
                    e => e.Username, e => e.Email, e => e.LastName, e => e.FirstName, e => e.AccountCreated, e => e.Extension, 
                    e => e.PhoneNumber, e => e.HireDate, e => e.Nickname, e => e.EmployeeNumber, e => e.PhotoPath,
                    e => e.JobTitleId, e => e.DepartmentId, e => e.ManagerId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employeeToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save after multiple attempts.");
                }
                catch (DbUpdateException ex)
                {
                    string err = ex.GetBaseException().Message;
                    if (err.Contains("unique") && err.Contains("ix_employee_username"))
                    {
                        ModelState.AddModelError("Username", "Unable to save duplicate username.");
                    }
                    else if (err.Contains("unique") && err.Contains("ix_employee_email"))
                    {
                        ModelState.AddModelError("Username", "Unable to save duplicate email.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save for some reason");
                    }
                }
            }
            PopulateDropdownLists(employeeToUpdate);
            return View(employeeToUpdate);
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
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Manager)
                .Include(e => e.EmployeeLocations).ThenInclude(el => el.Location)
                .FirstOrDefaultAsync(e => e.Id == id);

            try
            {
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if(ex.GetBaseException().Message.Contains("The deleted statement conflicted with reference constraint"))
                {
                    ModelState.AddModelError("", "Unable to delete record. Foreign key issue, I think");
                }
                else if(ex.GetBaseException().Message.Contains("The DELETE statement conflicted with the SAME TABLE REFERENCE constraint"))
                {
                    ModelState.AddModelError("", "Cannot delete employee who is set as manager. I think");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to detele the record. I dont know all the possible errors.");
                }
            }

            return View(employee);

            
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

            foreach (var option in allOptions)
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
            var managerIds = _context.Employees.Where(e => e.ManagerId != null).Select(e => e.ManagerId).Distinct().ToList();
            List<Employee> managers = new();

            foreach (var tempId in managerIds)
            {
                var manager = _context.Employees.FirstOrDefault(e => e.Id == tempId);
                managers.Add(manager);
            }

            return new SelectList(managers.OrderBy(m => m.LastName), "Id", "Summary", managerId);
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
