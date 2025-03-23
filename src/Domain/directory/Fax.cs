using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Domain.extension;

namespace Domain.directory;

public class Fax : AuditableEntity, IValidatableObject
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Display(Name = "Fax")]
    public string FaxFormatted => "(" + FaxNumber?.Substring(0, 3) + ") " + FaxNumber?.Substring(3, 3) + "-" + FaxNumber?[6..];


    [Display(Name = "Fax Name")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Required(ErrorMessage = "Cannot leave {0} blank")]
    public string FaxName { get; set; } = "";


    [Display(Name = "Fax Number")]
    [Required(ErrorMessage = "Cannot leave {0} blank")]
    [Phone]
    public string FaxNumber { get; set; } = "";



    [Display(Name = "Is Fax Forwarded")]
    public bool IsForwarded { get; set; }

    [Display(Name = "Forwarded To")]
    [DataType(DataType.PhoneNumber)]
    public string? ForwardTo { get; set; }




    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }



    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(IsForwarded)
        {
            if(ForwardTo == null)
            {
                yield return new ValidationResult("Need forward number if fax has been forwarded");
            }
        }
        else
        {
            if(ForwardTo != null)
            {
                yield return new ValidationResult("Remove forward number if the fax is not forwaded");
            }
        }
    }
}
