using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.extension;

namespace Domain.directory;

public class Employee : AuditableEntity, IValidatableObject
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }


    #region Summary

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
    public string? PhoneFormatted
    {
        get
        {
            if(PhoneNumber != null)
            {
                return "(" + PhoneNumber?[..3] + ") " + PhoneNumber?.Substring(3, 3) + "-" + PhoneNumber?[6..];
            }
            return PhoneNumber;
        }
    }

    [Display(Name = "Created")]
    public DateOnly AccountCreatedDate
    {
        get
        {
            return DateOnly.FromDateTime(AccountCreated);
        }
    }

    #endregion

    #region LDAP Properties
    // from AD - can't modify
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    [Required(ErrorMessage = "Cannot leave {0} blank")]
    public string Username { get; set; } = "";

    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
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

    #endregion

    #region Remaining Properties
    //from AD but modify from webapp
    [StringLength(5, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Extension { get; set; }
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }



    //in the future -DateTime not nullable from fluent API
    [Display(Name = "Hire Date")]
    public DateOnly? HireDate { get; set; }
    public string? Nickname { get; set; }
    [MaxLength(20)]
    [Display(Name = "Employee Number")]
    public string? EmployeeNumber { get; set; }

    #endregion

    //manage concurrency
    [ScaffoldColumn(false)]
    [Timestamp]
    public Byte[]? RowVersion { get; set; }


    #region Foreign Key

    //one-many nullable foreign key
    public int? JobTitleId { get; set; }
    [Display(Name = "Job Title")]
    public JobTitle? JobTitle { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }



    //many-many foreign key with Junction table
    [Display(Name = "Locations")]
    public ICollection<EmployeeLocation> EmployeeLocations { get; set; } = new HashSet<EmployeeLocation>();

    [Display(Name = "Documents")]
    public ICollection<EmployeeDocument> EmployeeDocuments { get; set; } = new HashSet<EmployeeDocument>();



    //one-one foreign key
    public EmployeePhoto? EmployeePhoto { get; set; }
    public EmployeeThumbnail? EmployeeThumbnail { get; set; }


    //Self-Referencing foreign key
    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }


    //Two-way self referencing foreign key loop
    [Display(Name = "Direct Reports")]
    public ICollection<Employee> PrimaryStaff { get; set; } = new HashSet<Employee>();

    #endregion


    //IValidation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        DateTime today = DateTime.Now;
        DateOnly dateOnly = DateOnly.FromDateTime(AccountCreated);

        if(AccountCreated > today)
        {
            yield return new ValidationResult("Hire date cannot be future");
        }

        if (dateOnly.CompareTo(HireDate) < 0)
        {
            yield return new ValidationResult("Account cannot be created before hire date");
        }
        
    }
}
