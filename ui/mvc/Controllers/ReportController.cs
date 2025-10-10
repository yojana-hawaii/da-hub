using Domain.directoryViewModel;
using Infrastructure.dbcontext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.CustomController;
using mvc.ViewModel;
using mvc.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace mvc.Controllers
{
    public class ReportController : CognizantController
    {
        private readonly DirectoryContext _context;
        public ReportController(DirectoryContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult DownloadEmployees()
        {
            var emp = from e in _context.Employees
                      .Include(j => j.JobTitle)
                      .Include(d => d.Department)
                      .Include(el => el.EmployeeLocations).ThenInclude(l => l.Location)
                      .AsNoTracking()
                      orderby e.AccountCreated descending
                      // properties become excel header. underscore converts to space
                      select new
                      {
                          Date = e.AccountCreated.ToShortDateString(),
                          Employee = e.Summary,
                          e.Extension,
                          Phone = e.PhoneFormatted,
                          JobTitle = e.JobTitle == null ? null :  e.JobTitle.JobTitleName,
                          Department = e.Department == null ? null : e.Department.DepartmentName,
                          e.Email // will create Email property in the anonymous type created here
                      };
            int rowCount = emp.Count();

            if (rowCount > 0)
            {
                // If you use EPPlus for Noncommercial personal use.
                ExcelPackage.License.SetNonCommercialPersonal("Yojana"); //This will also set the Author property to the name provided in the argument.


                // Option 1: start with existing document, upload it and add data to it
                /*
                var spreadsheet = _context.UploadedFiles
                    .Include(f => f.UploadedFileContent)
                    .Where(f => f.Id == 1)
                    .SingleOrDefault();
                using(MemoryStream memoryStream = new MemoryStream(spreadsheet.UploadedFileContent.Content))
                {
                    ExcelPackage package = new ExcelPackage(memoryStream);
                }
                */

                // Option 2: Create excel file from scratch
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var worksheet = excel.Workbook.Worksheets.Add("Employees"); // worksheet name

                    // individual cell
                    // Cells[row, column] - row 3, column 1 - leave first 2 row blank
                    worksheet.Cells[3, 1].LoadFromCollection(emp, true); // print header true

                    // entire column
                    // column format
                    worksheet.Column(1).Style.Numberformat.Format = "mm-dd-yyyy";
                    worksheet.Column(3).Style.Numberformat.Format = "0";

                    // block of cells
                    // Cells[startRow, startColumn, endRow, EndColumn] > make first 2 column bold
                    worksheet.Cells[4, 1, rowCount + 3, 2].Style.Font.Bold = true;

                    // multiple action to one or more cells > add summary at the bottom of the 3rd column
                    using(ExcelRange totalPhone = worksheet.Cells[rowCount + 4, 3])
                    {
                        // adding extensions does not make sense here but testing using formula and convert it to $$$
                        totalPhone.Formula = "Sum(" + worksheet.Cells[4, 3].Address + ":" + worksheet.Cells[rowCount + 3, 3].Address + ")";
                        totalPhone.Style.Font.Bold = true;
                        totalPhone.Style.Numberformat.Format = "$###,##0.00";
                    }
                    
                    // adding email into comment  
                    for (int i = 4; i < rowCount + 4; i++)
                    {
                        using (ExcelRange range = worksheet.Cells[i, 7])
                        {
                            string? email =  range.Value.ToString();
                            int length = email?.Length < 5 ? email.Length : 5;
                            string partial = new string(email?.Take(length).ToArray());
                            range.Value = partial + "..."; 

                            ExcelComment comment = range.AddComment(email, "email");
                            comment.AutoFit = true;
                        }
                    }

                    // background color on heading > all 7 heading columns
                    using (ExcelRange headings = worksheet.Cells[3, 1, 3, 7])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    // autofit
                    worksheet.Cells.AutoFitColumns();

                    // add title in merged cells and centered alignment.
                    worksheet.Cells[1, 1].Value = "Employee Report";
                    using(ExcelRange range = worksheet.Cells[1,1,1,6])
                    {
                        range.Merge = true;
                        range.Style.Font.Bold = true;
                        range.Style.Font.Size = 18;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    //add timestamp
                    DateTime utc = DateTime.UtcNow;
                    TimeZoneInfo hst = TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utc, hst);

                    using(ExcelRange range = worksheet.Cells[2, 6])
                    {
                        range.Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                        range.Style.Font.Bold = true;
                        range.Style.Font.Size = 13;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    try
                    {
                        Byte[] data = excel.GetAsByteArray();
                        string filename = "Employees.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(data, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Count not build and download the file");
                    }
                }
            }

            return NotFound();
        }

        public IActionResult DepartmentSummaryLinq()
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .GroupBy(ed => new { ed.DepartmentId, ed.Department.DepartmentName })
                .Select(grp => new DepartmentSummaryVM
                {
                    Id = (int)(grp.Key.DepartmentId == null ? 0 : grp.Key.DepartmentId),
                    DepartmentName = grp.Key.DepartmentName == null ? "No Department" : grp.Key.DepartmentName,
                    EmployeeCount = grp.Count()
                })
                .OrderBy(d => d.DepartmentName);
            return View(query.AsNoTracking().ToList());
        }
        public async Task<IActionResult> ManagerSummaryView(int? page, int? pageSizeId)
        {
            var query = _context.ManagerSummaries
                .OrderBy(a => a.LastName)
                .ThenBy(mngr => mngr.FirstName)
                .AsNoTracking();

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeId, "ManagerSummaryView");
            ViewData["pageSizeId"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<ManagerSummaryVM>
                .CreateAsync(query.AsNoTracking(), page ?? 1, pageSize);

            return View(query);
        }
    }
}
