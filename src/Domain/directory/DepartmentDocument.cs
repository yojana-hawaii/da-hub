using System.ComponentModel.DataAnnotations;

namespace Domain.directory;

public class DepartmentDocument : UploadedFile
{
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    public Department? Department { get; set; }
}
