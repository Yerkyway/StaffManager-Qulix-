namespace StaffManager.Models;

public enum Position
{
    Manager,
    Developer,
    BusinessAnalyst,
    Tester,
}

public class EmployeeModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public Position Position { get; set; }
    public DateTime HireDate { get; set; }
    public CompanyModel Company { get; set; }
}