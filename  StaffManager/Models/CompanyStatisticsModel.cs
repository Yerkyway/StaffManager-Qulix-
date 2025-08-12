namespace StaffManager.Models;

public class CompanyStatisticsModel
{
    public int TotalCompanies { get; set; }
    public int CompaniesWithEmployees { get; set; }
    public int CompaniesWithoutEmployees { get; set; }
    public double AverageEmployeesPerCompany { get; set; }
    public int LargestCompanySize { get; set; }
    public Dictionary<string, int> MostCommonLegalForms { get; set; }
}