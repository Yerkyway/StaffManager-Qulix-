namespace StaffManager.Models;

public class EmployeeStatisticsModel
{
    public int TotalEmployees { get; set; }
    public Dictionary<string, int> EmployeesByPosition { get; set; }
    public double AverageWorkExperienceYears { get; set; }
    public int NewEmployeesThisYear { get; set; }
    public EmployeeModel LongestWorkingEmployee { get; set; }
}