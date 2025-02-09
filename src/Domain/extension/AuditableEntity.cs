namespace Domain.extension;

public class AuditableEntity
{
    public string? CreatedBy { get; set; }
    public string? MOdifiedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
