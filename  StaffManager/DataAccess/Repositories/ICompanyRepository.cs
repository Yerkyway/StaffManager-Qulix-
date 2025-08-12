using StaffManager.Models;

namespace StaffManager.Repository;

public interface ICompanyRepository
{
    Task<List<CompanyModel>> GetAllCompaniesAsync();
    Task<CompanyModel> GetCompanyByIdAsync(int id);
    Task<int> createCompanyAsync(CompanyModel company);
    Task UpdateCompanyAsync(CompanyModel company);
    Task DeleteCompanyAsync(int id);
}