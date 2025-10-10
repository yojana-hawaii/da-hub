using System.ComponentModel.DataAnnotations;

namespace Domain.directoryViewModel;

public class ManagerSummaryVM
{
    public int Id { get; set; }
    [Display(Name = "Full Name")]
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Direct Report Count")]
    public int NumberOfStaff { get; set; }

    [Display(Name = "Most Recent Hire Date")]
    public DateOnly LatestHireDate { get; set; }

    [Display(Name = "Oldest Hire Date")]
    public DateOnly OldestHireDate { get; set; }
}
