using StaffManager.Models;

namespace StaffManager.Repository;

public interface IEmployeeRepository
{
    Task<List<EmployeeModel>> GetAllEmployeesAsync();
    Task<EmployeeModel> GetEmployeeByIdAsync(int id);
    Task<int> CreateEmployeeAsync(EmployeeModel employee);
    Task UpdateEmployeeAsync(EmployeeModel employee);
    Task DeleteEmployeeAsync(int id);
    
    Task<List<EmployeeModel>> GetEmployeesByCompanyIdAsync(int companyId);
}