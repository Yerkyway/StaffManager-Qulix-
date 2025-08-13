using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffManager.Models;

public enum Position
{
    None = 0,
    Manager = 1,
    Developer = 2,
    BusinessAnalyst = 3,
    Tester = 4,
}

public class EmployeeModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    
    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Position is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid position.")]
    public Position Position { get; set; }
    
    [Required(ErrorMessage = "Hire date is required.")]
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }
    
    [Required(ErrorMessage = "Company is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid company.")]
    public int CompanyId { get; set; } 
    [ForeignKey(nameof(CompanyId))]
    public CompanyModel? Company { get; set; }
}