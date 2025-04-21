using Domain.extension;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.directory;

public class Department : AuditableEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Cannot leave {0} blank")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = "";



    public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    public ICollection<Fax> Faxes { get; set; } = new HashSet<Fax>();

    [Display(Name = "Documents")]
    public ICollection<DepartmentDocument> DepartmentDocuments { get; set; } = new HashSet<DepartmentDocument>();

}
