using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.directory;

public class UploadedFileContent
{
    //primary key is foreign key and same as primary key of UploadedFile > offload Content property which can be massive
    [Key, ForeignKey("UploadedFile")]
    public int FileContentId { get; set; }


    [ScaffoldColumn(false)]
    public byte[]? Content { get; set; }


    public UploadedFile? UploadedFile { get; set; }

}
