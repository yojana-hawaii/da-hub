using Domain.extension;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.directory;

public class Department : AuditableEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Display(Name = "Department Name")]
    public required string DepartmentName { get; set; }



    public IEnumerable<Employee> Employees { get; set; } = new HashSet<Employee>();
    public IEnumerable<Fax> Faxes { get; set; } = new HashSet<Fax>();
}
