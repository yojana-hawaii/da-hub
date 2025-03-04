using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.extension;

namespace Domain.directory;

public class JobTitle : AuditableEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Cannot leave {0} blank")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Display(Name = "Job Title")]
    public string JobTitleName { get; set; } = "";

    [StringLength(500, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Display(Name = "Job Description")]
    public string? JobTitleDescription { get; set; }



    //Two-way foreign key loop
    public IEnumerable<Employee> Employees { get; set; } = new HashSet<Employee>();
}
