using System.ComponentModel.DataAnnotations;

namespace Domain.directory;

// separate for image and not using UploadedFile and UploadedFileContent 
// no advantage but can lead to complications
public class EmployeePhoto
{
    public int Id { get; set; }

    [ScaffoldColumn(false)]
    public byte[]? Content { get; set; }

    [StringLength(255)]
    public string? MimeType { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Emmployee { get; set; }
}
