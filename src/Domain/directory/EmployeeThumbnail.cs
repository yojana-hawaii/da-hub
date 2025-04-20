using System.ComponentModel.DataAnnotations;

namespace Domain.directory;

public class EmployeeThumbnail
{

    public int Id { get; set; }

    [ScaffoldColumn(false)]
    public byte[]? Content { get; set; }

    [StringLength(255)]
    public string? MimeType { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Emmployee { get; set; }
}
