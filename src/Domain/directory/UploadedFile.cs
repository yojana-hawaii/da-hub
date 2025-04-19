using System.ComponentModel.DataAnnotations;

namespace Domain.directory;

public class UploadedFile
{
    public int Id { get; set; }
    [StringLength(255, ErrorMessage = "The name of the file cannot exceed 255 characters.")]
    [Display(Name = "File Name")]
    public string? FileName { get; set; }


    [StringLength(255)]
    public string? MimeType { get; set; }

    public UploadedFileContent? UploadedFileContent { get; set; } = new UploadedFileContent();

}
