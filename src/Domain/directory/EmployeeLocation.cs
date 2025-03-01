using Domain.extension;

namespace Domain.directory;

public class EmployeeLocation : AuditableEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public int LocationId { get; set; }
    public Location? Location { get; set; }
}
