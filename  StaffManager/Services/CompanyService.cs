using StaffManager.Models;
using StaffManager.Repository;

namespace StaffManager.Services;


/// <summary>
/// defines the service for managing companies.
/// </summary>
public class CompanyService
{
    /// <summary>
    /// defines the repository for managing companies and employees.
    /// </summary>
    private readonly ICompanyRepository _companyRepository;
    private readonly IEmployeeRepository _employeeRepository;
    
    public CompanyService(ICompanyRepository companyRepository, IEmployeeRepository employeeRepository)
    {
        _companyRepository = companyRepository;
        _employeeRepository = employeeRepository;
    }
    
    /// <summary>
    /// defines the legal forms of companies.
    /// </summary>

    private readonly string[] _allowedLegalForms =
    {
        "ООО", "ЗАО", "ОАО", "ИП", "АО", "ПАО", "НКО", "ГУП", "МУП"
    };
    
    /// <summary>
    /// defines the method to get all companies.
    /// </summary>
    public async Task<List<CompanyModel>> getAllCompaniesAsync()
    {
        try
        {
            return await _companyRepository.GetAllCompaniesAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Ошибка при получении списка компаний", e);
        }
    }
    
    /// <summary>
    /// defines the method to get a company by its ID.
    /// </summary>
    public async Task<CompanyModel> getCompanyByIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }
        
        try
        {
            return await _companyRepository.GetCompanyByIdAsync(id);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Ошибка при получении компании с ID {id}", e);
        }
    }

    /// <summary>
    /// defines the method to create a company with validation of entered data.
    /// </summary>
    /// <param name="company"></param>
    public async Task<int> createCompanyAsync(CompanyModel company)
    {
        if (company==null)
        {
            throw new ArgumentNullException(nameof(company));
        }
        
        var (isValid, errors) = await ValidateCompanyAsync(company);
        if (!isValid)
        {
            throw new ArgumentException("Некорректные данные компании: " + string.Join(", ", errors));
        }

        try
        {
            company.Name = company.Name?.Trim();
            company.LegalForm = company.LegalForm?.Trim();

            return await _companyRepository.createCompanyAsync(company);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Ошибка при создании компании", e);
        }
    }
    
    /// <summary>
    /// defines the method to update a company with validation of entered data.
    /// </summary>
    public async Task updatedCompanyAsync(CompanyModel company)
    {
        if (company == null)
        {
            throw new ArgumentNullException(nameof(company));
        } else if (company.Id <= 0)
        {
            throw new ArgumentException("ID компании должен быть больше нуля", nameof(company.Id));
        }
        
        var existingCompany = await _companyRepository.GetCompanyByIdAsync(company.Id);
        if (existingCompany == null)
        {
            throw new InvalidOperationException("Компания не найдена");
        }
        
        var (isValid, errors) = await ValidateCompanyAsync(company);
        if (!isValid)
        {
            throw new ArgumentException("Некорректные данные компании: " + string.Join(", ", errors));
        }

        try
        {
            company.Name = company.Name?.Trim();
            company.LegalForm = company.LegalForm?.Trim();

            await _companyRepository.UpdateCompanyAsync(company);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Ошибка при обновлении компании с ID {company.Id}", e);
        }
    }

    
    /// <summary>
    /// defines the method to delete a company by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<bool> deleteCompanyAsync(int id)
    {
        if (id<=0)
        {
            return false;
        }

        try
        {
            var existingCompany = await _companyRepository.GetCompanyByIdAsync(id);
            if (existingCompany == null)
            {
                return false; 
            }

            var hasEmployees = await _companyRepository.HasEmployeesAsync(id);
            if (hasEmployees)
            {
                throw new InvalidOperationException("Невозможно удалить компанию, пока в ней есть сотрудники.");
            }
            
            await _companyRepository.DeleteCompanyAsync(id);
            return true;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Ошибка при удалении компании с ID {id}", e);
        }
    }
    
    
    /// <summary>
    /// defines the method to validate a company.
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    public async Task<(bool isValid, List<string> errors)> ValidateCompanyAsync(CompanyModel company)
    {
        var errors = new List<string>();

        if (company == null)
        {
            errors.Add("Компания не может быть null.");
            return (false, errors);
        }  
        if (string.IsNullOrWhiteSpace(company.Name))
        {
            errors.Add("Название компании не может быть пустым.");
        }
        else
        {
            if (company.Name.Trim().Length < 3 || company.Name.Trim().Length > 100)
            {
                errors.Add("Название компании должно быть от 3 до 100 символов.");
            }

            var companies = await _companyRepository.GetAllCompaniesAsync();
            var duplicateName = companies.Any(c => c.Id != company.Id && 
                                                   string.Equals(c.Name.Trim(), 
                                                       company.Name.Trim(), 
                                                       StringComparison.OrdinalIgnoreCase));
            if (duplicateName)
            {
                errors.Add("Компания с таким названием уже существует.");
            }
        }

        if (string.IsNullOrWhiteSpace(company.LegalForm))
        {
            errors.Add("Юридическая форма компании не может быть пустой.");
        }
        else
        {
            if (!_allowedLegalForms.Contains(company.LegalForm.Trim().ToUpper()))
            {
                errors.Add("Недопустимая юридическая форма компании. " +
                            "Допустимые формы: " + string.Join(", ", _allowedLegalForms));
            }
        }
        
        return (errors.Count == 0, errors);
    }
    
    /// <summary>
    /// defines the method to get statistics about companies.
    /// </summary>
    public async Task<CompanyStatisticsModel> GetCompanyStatisticsAsync(int companyId)
    {
        try
        {
            var companies = await _companyRepository.GetAllCompaniesAsync();
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            
            return new CompanyStatisticsModel
            {
                TotalCompanies = companies.Count,
                CompaniesWithEmployees = companies.Count(c => c.NumberOfEmployees > 0),
                CompaniesWithoutEmployees = companies.Count(c => c.NumberOfEmployees == 0),
                AverageEmployeesPerCompany = companies.Count > 0 ? (double)employees.Count / companies.Count : 0,
                LargestCompanySize = companies.Count > 0 ? companies.Max(c => c.NumberOfEmployees) : 0,
                MostCommonLegalForms = companies.GroupBy(c => c.LegalForm)
                    .OrderByDescending(g => g.Count())
                    .Take(3)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Ошибка при получении статистики компаний", e);
        }
    }
}