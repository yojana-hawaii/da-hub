using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.extension;

namespace Domain.directory;

public class Employee : AuditableEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    // from AD - can't modify
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public required string Username { get; set; }

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }
    public DateTime AccountCreated { get; set; }

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public required string FirstName { get; set; }
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public required string LastName { get; set; }


    //from AD but modify from webapp
    [StringLength(5, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Extension { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    //for search
    [StringLength(500, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Keyword { get; set; }



    //in the future -DateTime not nullable from fluent API
    public DateTime? HireDate { get; set; }
    public string? NickName { get; set; }
    [MaxLength(20)]
    public string? EmployeeNumber { get; set; }
    public string? PhotoPath { get; set; }



    //Foreign key
    public int? JobTitleId { get; set; }
    public JobTitle? JobTitle { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    //many to many with location
    [Display(Name = "Locations")]
    public ICollection<EmployeeLocation> EmployeeLocations { get; set; } = new HashSet<EmployeeLocation>();

    //Self-Referencing foreign key
    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }




    //Two-way foreign key loop
    [Display(Name = "Direct Reports")]
    public IEnumerable<Employee> PrimaryStaff { get; set; } = new HashSet<Employee>();
}
