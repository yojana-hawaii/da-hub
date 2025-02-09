using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.directory;

public class Location
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Location name is required. Example: location could be building name & sub-location can be floors in the building.")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public required string LocationName { get; set; }


    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? SubLocation { get; set; }



    public IEnumerable<Employee> Employees { get; set; } = new HashSet<Employee>();
    public IEnumerable<Fax> Faxes { get; set; } = new HashSet<Fax>();
}
