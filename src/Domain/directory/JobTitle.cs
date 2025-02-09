using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.directory;

public class JobTitle
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public required string JobTitleName { get; set; }

    [StringLength(500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? JobTitleDescription { get; set; }



    //Two-way foreign key loop
    public IEnumerable<Employee> Employees { get; set; } = new HashSet<Employee>();
}
