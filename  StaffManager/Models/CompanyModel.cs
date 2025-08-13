using System.ComponentModel.DataAnnotations;

namespace StaffManager.Models;

public class CompanyModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(100, 
        MinimumLength = 3, 
        ErrorMessage = "Company name must be between 3 and 100 characters.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Legal Form is required.")]
    public string LegalForm { get; set; }
    public int NumberOfEmployees { get; set; }
    
}