using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Domain.extension;

public class AuditableEntity
{
    [StringLength(100)]
    public string? CreatedBy { get; set; }
    [StringLength(100)]
    public string? ModifiedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
