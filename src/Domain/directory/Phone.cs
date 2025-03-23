using Domain.extension;
using System.ComponentModel.DataAnnotations;

namespace Domain.directory;

public class Phone : AuditableEntity
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Cannot leave {0} blank")]

    public string PhoneNumber { get; set; } = "";
    public string? Vendor { get; set; }
    public string? PhoneType { get; set; }
}
