using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Domain.extension;

public class AuditableEntity : IAuditable
{
    [ScaffoldColumn(false)]
    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    [ScaffoldColumn(false)]
    public string? ModifiedBy { get; set; }

    [ScaffoldColumn(false)]
    public DateTime CreatedDate { get; set; }

    [ScaffoldColumn(false)]
    public DateTime ModifiedDate { get; set; }
}
