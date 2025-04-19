using System.ComponentModel.DataAnnotations;

namespace Domain.directory;

public class EmployeeDocument : UploadedFile
{
    [Display(Name = "Employee")]
    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }
}
