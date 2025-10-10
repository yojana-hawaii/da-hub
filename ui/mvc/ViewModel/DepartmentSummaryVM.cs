using System.ComponentModel.DataAnnotations;

namespace mvc.ViewModel;

public class DepartmentSummaryVM
{
    public int Id { get; set; }
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;

    [Display(Name = "Employee Count")]
    public int EmployeeCount { get; set; }
}
