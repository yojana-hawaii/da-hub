using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Domain.extension;

namespace Domain.directory;

public class Fax : AuditableEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    [Display(Name = "Fax Name")]
    [Required(ErrorMessage = "Fax name is required.")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public required string FaxName { get; set; }


    [Required(ErrorMessage = "Fax number is required.")]
    [DataType(DataType.PhoneNumber)]
    public required string FaxNumber { get; set; }


    public bool FaxForward { get; set; }
    public int? ForwardTo { get; set; }




    public int LocationId { get; set; }
    public Location? Location { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
}
