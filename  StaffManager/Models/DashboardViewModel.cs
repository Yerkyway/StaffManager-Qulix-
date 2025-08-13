namespace StaffManager.Models;

public class DashboardViewModel
{
    public int totalCompanies { get; set; }
    public int totalEmployees { get; set; }
    public int companiesWithEmployees { get; set; }
    public List<EmployeeModel> recentEmployees { get; set; } = new List<EmployeeModel>();
}