using StaffManager.Models;
using StaffManager.Repository;

namespace StaffManager.Services;

public class EmployeeService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IEmployeeRepository _employeeRepository;
    
    public EmployeeService(ICompanyRepository companyRepository, IEmployeeRepository employeeRepository)
    {
        _companyRepository = companyRepository;
        _employeeRepository = employeeRepository;
    }

    
    /// <summary>
    /// Получение списка всех сотрудников
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<EmployeeModel>> GetAllEmployeesAsync()
    {
        try
        {
            return await _employeeRepository.GetAllEmployeesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Ошибка при получении списка сотрудников", e);
        }
    }

    
    /// <summary>
    /// Получение сотрудника по ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<EmployeeModel> GetEmployeeByIdAsync(int id)
    {
        if (id<=0)
        {
            return null;
        }

        try
        {
            return await _employeeRepository.GetEmployeeByIdAsync(id);
        }
        catch (Exception e)
        {
            throw new Exception($"Ошибка при получении сотрудника с ID {id}", e);
        }
    }

    /// <summary>
    /// Создание нового сотрудника
    /// </summary>
    public async Task<int> CreateEmployeeAsync(EmployeeModel employee)
    {
        if (employee == null)
        {
            throw new ArgumentNullException(nameof(employee), "Сотрудник не может быть null");
        }
        
        var (isValid, errors) = await ValidateEmployeeAsync(employee);
        if (!isValid)
        {
            throw new ArgumentException("Некорректные данные сотрудника: " + string.Join(", ", errors));
        }

        try
        {
            employee.FirstName = employee.FirstName?.Trim();
            employee.MiddleName = employee.MiddleName?.Trim();
            employee.LastName = employee.LastName?.Trim();

            return await _employeeRepository.CreateEmployeeAsync(employee);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Ошибка при создании сотрудника", e);
        }
    }
    
    /// <summary>
    /// Обновление данных сотрудника
    /// </summary>
    public async Task UpdateEmployeeAsync(EmployeeModel employee)
    {
        if (employee == null)
        {
            throw new ArgumentNullException(nameof(employee), "Сотрудник не может быть null");
        } 
        else if (employee.Id <= 0)
        {
            throw new ArgumentException("ID сотрудника должен быть больше нуля", nameof(employee.Id));
        }

        var existingEmployee = await _employeeRepository.GetEmployeeByIdAsync(employee.Id);
        if (existingEmployee == null)
        {
            throw new KeyNotFoundException($"Сотрудник с ID {employee.Id} не найден");
        }

        var (isValid, errors) = await ValidateEmployeeAsync(employee);
        if (!isValid)
        {
            throw new ArgumentException("Некорректные данные сотрудника: " + string.Join(", ", errors));
        }

        try
        {
            employee.FirstName = employee.FirstName?.Trim();
            employee.MiddleName = employee.MiddleName?.Trim();
            employee.LastName = employee.LastName?.Trim();

            await _employeeRepository.UpdateEmployeeAsync(employee);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Ошибка при обновлении сотрудника", e);
        }
    }

    /// <summary>
    /// deletes an employee by ID.
    /// </summary>
    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        if (id <=0)
        {
            return false;
        }

        try
        {
            var existingEmployee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                return false; 
            }

            await _employeeRepository.DeleteEmployeeAsync(id);
            return true;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Ошибка при удалении сотрудника с ID {id}", e);
        }
    }

    public async Task<List<EmployeeModel>> GetEmployeeByCompanyIdAsync(int companyId)
    {
        if (companyId <= 0)
        {
            return new List<EmployeeModel>();
        }

        try
        {
            return await _employeeRepository.GetEmployeesByCompanyIdAsync(companyId);
        }
        catch (Exception e)
        {
            throw new Exception($"Ошибка при получении сотрудников компании с ID {companyId}", e);
        }
    }
    
    /// <summary>
    /// validates the employee data.
    /// </summary>
    public async Task<(bool isValid, List<string> errors)> ValidateEmployeeAsync(EmployeeModel employee)
    {
        var errors = new List<string>();

        if (employee == null)
        {
            errors.Add("Сотрудник не может быть null");
            return (false, errors);
        }

        if (string.IsNullOrWhiteSpace(employee.FirstName))
        {
            errors.Add("Имя сотрудника не может быть пустым");
        } 
        else if (employee.FirstName.Trim().Length < 2 || employee.FirstName.Trim().Length > 50)
        {
            errors.Add("Имя сотрудника должно быть от 2 до 50 символов");
        }

        if (string.IsNullOrWhiteSpace(employee.LastName))
        {
            errors.Add("Фамилия сотрудника не может быть пустой");
        } 
        else if (employee.LastName.Trim().Length < 2 || employee.LastName.Trim().Length > 50)
        {
            errors.Add("Фамилия сотрудника должна быть от 2 до 50 символов");
        }

        if (!Enum.IsDefined(typeof(Position), employee.Position) || (int)employee.Position == 0)
        {
            errors.Add("Указана некорректная должность");
        }

        if (employee.HireDate == default)
        {
            errors.Add("Дата найма сотрудника не может быть пустой");
        } 
        else if (employee.HireDate.Date > DateTime.Now)
        {
            errors.Add("Дата найма сотрудника не может быть в будущем");
        } 
        else if (employee.HireDate.Date < DateTime.Now.AddYears(-50).Date)
        {
            errors.Add("Дата найма сотрудника не может быть более 50 лет назад");
        }

        if (employee.CompanyId <= 0)
        {
            errors.Add("Необходимо выбрать компанию");
        }
        else
        {
            var existingCompany = await _companyRepository.GetCompanyByIdAsync(employee.CompanyId);
            if (existingCompany == null)
            {
                errors.Add($"Компания с ID {employee.CompanyId} не найдена");
            }
        }


        return (errors.Count == 0, errors);
    }
    
}