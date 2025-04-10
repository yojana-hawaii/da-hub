using Domain.directory;
using Infrastructure.dbcontext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using mvc.CustomController;
using mvc.Utilities;
using mvc.ViewModel;

namespace mvc.Controllers
{
    public class EmployeeController : ReturnUrlController
    {
        private readonly DirectoryContext _context;

        public EmployeeController(DirectoryContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index(string? searchString,
                                            int? JobTitleId, int? DepartmentId, int? ManagerId,
                                            int? LocationId,
                                            int? page, int? pageSizeId,
                                            string? actionButton, string sortDirection = "asc", string sortField = "Employee")
        {
            PopulateDropdownLists();

            //extra selectList for Locations
            ViewData["LocationDropdown"] = new SelectList(_context.Locations.OrderBy(l => l.LocationName), "Id", "Summary");

            //Start with include and make ur expression return IQuerable<Employee> so we can add filter and sort later
            var employees = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Manager)
                .Include(e => e.EmployeeLocations).ThenInclude(el => el.Location)
                .AsNoTracking();

            employees = FilterEmployee(searchString, JobTitleId, DepartmentId, LocationId, ManagerId, employees);

            //sumbit button named actionButton used for filter & sorting. Value is different and that is what comes in 
            string[] sortOptions = new[] { "Employee", "Email", "Phone", "Job Title", "Department" }; //names have to match table headeer to sort by

            if (!string.IsNullOrEmpty(actionButton)) // if form was submitted not new  page open
            {
                page = 1; //reset the page if any sorting us used
                if (sortOptions.Contains(actionButton)) //filter would be actionButton for filter/search but not in sort option define at the top
                {
                    if (actionButton == sortField) // reverse the order if same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; // sort by action button clicked
                }
            }



            var paginatedEmployees = await SortEmployeesAsync(employees, sortField, sortDirection, page, pageSizeId);

            return View(paginatedEmployees); // IQuerable executed when ToList is called
        }

        
        // GET: Employee/Details/5
        [Authorize]
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
        [Authorize]
        public IActionResult Create()
        {
            Employee employee = new();
            PopulateLocationMultiselectCheckbox(employee);
            PopulateDropdownLists();
            return View(employee);
        }


        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Username,Email,FirstName,LastName,Extension,PhoneNumber," +
                            "AccountCreated,NickName,EmployeeNumber,PhotoPath,JobTitleId,DepartmentId,ManagerId")] Employee employee, string[] selectedOptions)
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
                    //display change detail instead of going back to index
                    return RedirectToAction("Details", new { employee.Id });
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
            PopulateLocationMultiselectCheckbox(employee); // display selected checkbox even if there is validation error.
            PopulateDropdownLists(employee);
            return View(employee);
        }


        // GET: Employee/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
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
                .FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            PopulateDropdownLists(employee);
            PopulateLocationListBox(employee);
            return View(employee);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions, Byte[] RowVersion)
        {
            var employeeToUpdate = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Manager)
                .Include(e => e.EmployeeLocations).ThenInclude(el => el.Location)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employeeToUpdate == null)
            {
                return NotFound();
            }


            UpdateEmployeeLocationListboxCheckbox(selectedOptions, employeeToUpdate);

            //something may have change between GET and POST action. Start with RowVersion from GET 
            _context.Entry(employeeToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            if (await TryUpdateModelAsync<Employee>(employeeToUpdate, "",
                    e => e.Username, e => e.Email, e => e.LastName, e => e.FirstName, e => e.AccountCreated, e => e.Extension,
                    e => e.PhoneNumber, e => e.HireDate, e => e.Nickname, e => e.EmployeeNumber, e => e.PhotoPath,
                    e => e.JobTitleId, e => e.DepartmentId, e => e.ManagerId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    //display change detail instead of going back to index
                    return RedirectToAction("Details", new { employeeToUpdate.Id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Employee)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("", "Unable to save changes. Patient was delete by another user");
                    }
                    else
                    {
                        var databaseValue = (Employee)databaseEntry.ToObject();

                        if (databaseValue.Username != clientValues.Username)
                            ModelState.AddModelError("Username", "Curent Value: " + databaseValue.Username);

                        if (databaseValue.FirstName != clientValues.FirstName)
                            ModelState.AddModelError("FirstName", "Curent Value: " + databaseValue.FirstName);

                        if (databaseValue.LastName != clientValues.LastName)
                            ModelState.AddModelError("LastName", "Curent Value: " + databaseValue.LastName);

                        if (databaseValue.Extension != clientValues.Extension)
                            ModelState.AddModelError("Extension", "Curent Value: " + databaseValue.Extension);

                        if (databaseValue.PhoneNumber != clientValues.PhoneNumber)
                            ModelState.AddModelError("PhoneNumber", "Curent Value: " + databaseValue.PhoneFormatted);

                        if (databaseValue.Email != clientValues.Email)
                            ModelState.AddModelError("Email", "Curent Value: " + databaseValue.Email);

                        if (databaseValue.Nickname != clientValues.Nickname)
                            ModelState.AddModelError("Nickname", "Curent Value: " + databaseValue.Nickname);

                        if (databaseValue.EmployeeNumber != clientValues.EmployeeNumber)
                            ModelState.AddModelError("EmployeeNumber", "Curent Value: " + databaseValue.EmployeeNumber);

                        if (databaseValue.PhotoPath != clientValues.PhotoPath)
                            ModelState.AddModelError("PhotoPath", "Curent Value: " + databaseValue.PhotoPath);



                        if (databaseValue.AccountCreated != clientValues.AccountCreated)
                            ModelState.AddModelError("AccountCreated", "Curent Value: " + String.Format("{0:d}", databaseValue.AccountCreated));

                        if (databaseValue.HireDate != clientValues.HireDate)
                            ModelState.AddModelError("HireDate", "Curent Value: " + String.Format("{0:d}", databaseValue.HireDate));



                        //foreign key
                        if (databaseValue.DepartmentId != clientValues.DepartmentId)
                        {
                            if (databaseValue.DepartmentId.HasValue)
                            {
                                Department? dbDepartment = await _context.Departments.FirstOrDefaultAsync(d => d.Id == databaseValue.DepartmentId);
                                ModelState.AddModelError("DepartmentId", $"Curent Value:  {dbDepartment?.DepartmentName}");
                            }
                        }

                        if (databaseValue.JobTitleId != clientValues.JobTitleId)
                        {
                            if (databaseValue.JobTitleId.HasValue)
                            {
                                JobTitle? dbJob = await _context.JobTitles.FirstOrDefaultAsync(j => j.Id == databaseValue.JobTitleId);
                                ModelState.AddModelError("JobTitleId", $"Curent Value: {dbJob?.JobTitleName}");
                            }
                        }

                        if (databaseValue.ManagerId != clientValues.ManagerId)
                        {
                            if (databaseValue.ManagerId.HasValue)
                            {
                                Employee? dbMngr = await _context.Employees.FirstOrDefaultAsync(m => m.Id == databaseValue.ManagerId);
                                ModelState.AddModelError("ManagerId", $"Curent Value: {dbMngr?.Summary}");
                            }
                        }



                        if (databaseValue.EmployeeLocations != clientValues.EmployeeLocations)
                        {
                            ModelState.AddModelError("EmployeeLocations", "Curent Value: " + databaseValue.EmployeeLocations);

                        }


                        ModelState.AddModelError("", "The record you attempted to change was modified by somone else. The edit process has been paused. " +
                            "If you still want save your version click SAVE again.");


                        //update rowVersion with database value, and remove RowVErsion Error from model state
                        employeeToUpdate.RowVersion = databaseValue.RowVersion ?? Array.Empty<Byte>();
                        ModelState.Remove("RowVersion");
                    }

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save after multiple attempts.");
                }
                catch (DbUpdateException ex)
                {
                    string err = ex.GetBaseException().Message;
                    if (err.Contains("UNIQUE") && err.Contains("ix_employee_username"))
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
            PopulateLocationListBox(employeeToUpdate);
            PopulateDropdownLists(employeeToUpdate);
            return View(employeeToUpdate);
        }

        // GET: Employee/Delete/5
        [Authorize]
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
                .FirstOrDefaultAsync(m => m.Id == id); //FindAsync faster but only works with one entity.
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
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

                return RedirectToIndexAfterSaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.GetBaseException().Message.Contains("The deleted statement conflicted with reference constraint"))
                {
                    ModelState.AddModelError("", "Unable to delete record. Foreign key issue, I think");
                }
                else if (ex.GetBaseException().Message.Contains("The DELETE statement conflicted with the SAME TABLE REFERENCE constraint"))
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

        private IActionResult RedirectToIndexAfterSaveChanges()
        {
            var returnUrl = ViewData["returnUrl"]?.ToString();
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction(nameof(Index));
            }
            return Redirect(returnUrl);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        //filter and pagination
        private IQueryable<Employee> FilterEmployee(string? searchString, int? JobTitleId, int? DepartmentId, int? LocationId, int? ManagerId, IQueryable<Employee> employees)
        {
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            if (DepartmentId.HasValue)
            {
                employees = employees.Where(e => e.DepartmentId == DepartmentId);
                numberFilters++;
            }

            if (JobTitleId.HasValue)
            {
                employees = employees.Where(e => e.JobTitleId == JobTitleId);
                numberFilters++;
            }
            if (ManagerId.HasValue)
            {
                employees = employees.Where(e => e.ManagerId == ManagerId);
                numberFilters++;
            }

            if (LocationId.HasValue)
            {
                //dot any for many-many 
                employees = employees.Where(l => l.EmployeeLocations.Any(c => c.LocationId == LocationId));
                numberFilters++;
            }
            if ((!string.IsNullOrEmpty(searchString)))
            {
                employees = employees.Where(e =>
                                    e.LastName.ToUpper().Contains(searchString.ToUpper())
                                    || e.FirstName.ToUpper().Contains(searchString.ToUpper())
                                    || e.Extension.Contains(searchString.ToUpper())
                                    || e.PhoneNumber.Contains(searchString.ToUpper())
                                    || e.Username.ToUpper().Contains(searchString.ToUpper())

                                    );
                //employees = employees.Where(e => e.SearchKeywords.ToUpper().Contains(searchString.ToUpper()));
                numberFilters++;
            }

            if (numberFilters > 0)
            {
                ViewData["Filtering"] = " btn-danger ";
                ViewData["numberFilters"] = "(" + numberFilters.ToString() + " filter" + (numberFilters > 1 ? "s" : "") + " applied)";
                ViewData["showFilter"] = " show ";
            }


            return employees;
        }

        private async Task<PaginatedList<Employee>> SortEmployeesAsync(IQueryable<Employee> employees, string sortField, string sortDirection, int? page, int? pageSizeId)
        {
            if (sortField == "Employee")
            {
                if (sortDirection == "desc")
                {
                    employees = employees.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName);
                }
                else
                {
                    employees = employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);
                }
            }
            else if (sortField == "Email")
            {
                if (sortDirection == "desc")
                {
                    employees = employees.OrderByDescending(e => e.Email);
                }
                else
                {
                    employees = employees.OrderBy(e => e.Email);
                }
            }
            else if (sortField == "Phone")
            {
                if (sortDirection == "desc")
                {
                    employees = employees.OrderByDescending(e => e.Extension).ThenByDescending(e => e.PhoneNumber);
                }
                else
                {
                    employees = employees.OrderBy(e => e.Extension).ThenBy(e => e.PhoneNumber);
                }
            }
            else if (sortField == "Job Title")
            {
                if (sortDirection == "desc")
                {
                    employees = employees.OrderByDescending(e => e.JobTitle.JobTitleName);
                }
                else
                {
                    employees = employees.OrderBy(e => e.JobTitle.JobTitleName);
                }
            }
            else if (sortField == "Department")
            {
                if (sortDirection == "desc")
                {
                    employees = employees.OrderByDescending(e => e.Department.DepartmentName);
                }
                else
                {
                    employees = employees.OrderBy(e => e.Department.DepartmentName);
                }
            }
            else
            {
                employees = employees.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName);
            }

            //hidden field in Index view to track current sorting, just incase user clicks on same sorting, do the reverse. asc then desc then asc ...
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;


            //pagination
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeId, ControllerName());// CognizantController
            ViewData["pageSizeId"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Employee>.CreateAsync(employees.AsNoTracking(), page ?? 1, pageSize);


            return pagedData;
        }



        //UPDATE FOR LIST-BOX AND CHECK-BOX
        private void UpdateEmployeeLocationListboxCheckbox(string[] selectedOptions, Employee employeeToUpdate)
        {
            if (selectedOptions == null)
            {
                employeeToUpdate.EmployeeLocations = new List<EmployeeLocation>();
                return;
            }

            var _newOptions = new HashSet<string>(selectedOptions);
            var _currentOptions = new HashSet<int>(employeeToUpdate.EmployeeLocations.Select(el => el.LocationId));

            foreach (var loc in _context.Locations)
            {
                if (_newOptions.Contains(loc.Id.ToString())) //already selected
                {
                    if (!_currentOptions.Contains(loc.Id)) //selected but not Employee collection - Add it
                    {
                        employeeToUpdate.EmployeeLocations.Add(new EmployeeLocation
                        {
                            LocationId = loc.Id,
                            EmployeeId = employeeToUpdate.Id
                        });
                    }
                }
                else //not selected
                {
                    if (_currentOptions.Contains(loc.Id)) //not selected but in Employee Colection - Remove it
                    {
                        EmployeeLocation? locationToRemove = employeeToUpdate.EmployeeLocations.FirstOrDefault(e => e.LocationId == loc.Id);
                        if (locationToRemove != null)
                        {
                            _context.Remove(locationToRemove);
                        }
                    }
                }
            }
        }

        //LIST-BOXES
        private void PopulateLocationListBox(Employee employee)
        {
            var allOptions = _context.Locations;
            var currentOptions = new HashSet<int>(employee.EmployeeLocations.Select(e => e.LocationId));

            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();

            foreach (var loc in allOptions)
            {
                if (currentOptions.Contains(loc.Id))
                {
                    selected.Add(new ListOptionVM
                    {
                        Id = loc.Id,
                        DisplayText = loc.Summary
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        Id = loc.Id,
                        DisplayText = loc.Summary
                    });
                }
            }

            ViewData["LocationListBoxSelectedOption"] = new MultiSelectList(selected.OrderBy(l => l.DisplayText), "Id", "DisplayText");
            ViewData["LocationListBoxAvailableOption"] = new MultiSelectList(available.OrderBy(l => l.DisplayText), "Id", "DisplayText");
        }



        //CHECK-BOXES

        /// <summary>
        /// good for create and delete. Edit needs a little more
        /// allOptions has all locations, IsSelected is true only if current employee has locationId in their intersection table
        /// </summary>
        /// <param name="employee"></param>
        private void PopulateLocationMultiselectCheckbox(Employee employee)
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
            ViewData["LocationMultiSelectCheckbox"] = checkBoxes;
        }



        //DROPDOWN LISTS
        private void PopulateDropdownLists(Employee? employee = null)
        {
            ViewData["DepartmentDropdown"] = Departmentlist(employee?.DepartmentId);
            ViewData["JobTitleDropdown"] = JobTitleList(employee?.JobTitleId);
            ViewData["ManagerDropdown"] = ManagerList(employee?.ManagerId);
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
