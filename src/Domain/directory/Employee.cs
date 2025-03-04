using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.extension;

namespace Domain.directory;

public class Employee : AuditableEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Display(Name = "Name")]
    public string Summary
    {
        get
        {
            return LastName + ", " + FirstName;
        }
    }
    [Display(Name = "Search Keywords")]
    public string SearchKeywords
    {
        get
        {
            return FirstName + " " + LastName + " " + Extension + " " + PhoneNumber + " " + Username + " "
                + JobTitle?.JobTitleName + " " + Department?.DepartmentName + " " + EmployeeLocations.ToString();
        }
    }
    [Display(Name = "Phone")]
    public string PhoneFormatted => "(" + PhoneNumber?[..3] + ") " + PhoneNumber?.Substring(3, 3) + "-" + PhoneNumber?[6..];

    [Display(Name = "Created")]
    public DateOnly AccountCreatedDate
    {
        get
        {
            return DateOnly.FromDateTime(AccountCreated);
        }
    }


    // from AD - can't modify
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Required(ErrorMessage = "Cannot leave {0} blank")]
    public string Username { get; set; } = "";

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Cannot leave {0} blank")]
    public string Email { get; set; } = "";

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "Cannot leave {0} blank")]
    public string FirstName { get; set; } = "";

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Cannot leave {0} blank")]
    public string LastName { get; set; } = "";

    [Display(Name = "Created")]
    public DateTime AccountCreated { get; set; }


    //from AD but modify from webapp
    [StringLength(5, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Extension { get; set; }
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }



    //in the future -DateTime not nullable from fluent API
    [Display(Name = "Hire Date")]
    public DateTime? HireDate { get; set; }
    public string? Nickname { get; set; }
    [MaxLength(20)]
    [Display(Name = "Employee Number")]
    public string? EmployeeNumber { get; set; }
    public string? PhotoPath { get; set; }



    //Foreign key
    public int? JobTitleId { get; set; }
    [Display(Name = "Job Title")]
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
