using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using StaffManager.Models;
using StaffManager.Repository;

namespace StaffManager.DataAccess.RepositoryImplementation;

public class CompanyRepositoryImplementation : ICompanyRepository
{
    
    /// <summary>
    /// Represents a connection to the database.
    /// </summary>
    private readonly DatabaseConnection _databaseConnection;
    
    public CompanyRepositoryImplementation(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }
    
    /// <summary>
    /// Retrieves all companies from the database.
    /// </summary>
    /// <returns></returns>
    
    public async Task<List<CompanyModel>> GetAllCompaniesAsync()
    {
        var companies = new List<CompanyModel>();
        
        const string sql = @"
            SELECT c.Id, c.Name, c.LegalForm, 
                   count(e.Id) as EmployeeCount 
            FROM Companies c
            LEFT JOIN Employees e on c.Id = e.CompanyId
            GROUP BY c.Id, c.Name, c.LegalForm
            ORDER BY c.Name";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        companies.Add(new CompanyModel
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            LegalForm = reader.GetString("LegalForm"),
                            NumberOfEmployees = reader.GetInt32("NumberOfEmployees")
                        });
                    }
                }
            }
        }

        return companies;
    }
    
    
    /// <summary>
    /// Retrieves a company by its ID from the database.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<CompanyModel> GetCompanyByIdAsync(int id)
    {
        CompanyModel company = null;
        
        const string sql = @"
            SELECT c.Id, c.Name, c.LegalForm, 
                   count(e.Id) as EmployeeCount 
            FROM Companies c
            LEFT JOIN Employees e on c.Id = e.CompanyId
            WHERE c.Id = @Id
            GROUP BY c.Id, c.Name, c.LegalForm";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        company = new CompanyModel
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            LegalForm = reader.GetString("LegalForm"),
                            NumberOfEmployees = reader.GetInt32("NumberOfEmployees")
                        };
                    }
                }
            }
        }

        return company;
    }
    
    /// <summary>
    /// Creates a new company in the database.
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<int> createCompanyAsync(CompanyModel company)
    {
        if (company == null)
        {
            throw new ArgumentNullException(nameof(company));
        }
        
        const string sql = @"
            INSERT INTO Companies (Name, LegalForm)
            VALUES (@Name, @LegalForm);
            SELECT SCOPE_IDENTITY();";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 200) 
                    { Value = company.Name ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@LegalForm", SqlDbType.NVarChar, 50)
                    { Value = company.LegalForm ?? (object)DBNull.Value });
                
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
    }

    /// <summary>
    /// Updates an existing company in the database.
    /// </summary>
    /// <param name="company"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task UpdateCompanyAsync(CompanyModel company)
    {
        if (company == null)
        {
            throw new ArgumentNullException(nameof(company));
        }

        const string sql = @"
            UPDATE Companies
            SET Name = @Name, LegalForm = @LegalForm
            WHERE Id = @Id";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) 
                    { Value = company.Id });
                command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 200)
                    { Value = company.Name ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@LegalForm", SqlDbType.NVarChar, 50)
                    { Value = company.LegalForm ?? (object)DBNull.Value });

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    /// <summary>
    /// Deletes a company from the database by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task DeleteCompanyAsync(int id)
    {
        const string sql = @"
            DELETE FROM Companies
            WHERE Id = @Id";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)
                    {Value = id});

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    /// <summary>
    /// Checks if a company has any employees.
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    public async Task<bool> HasEmployeesAsync(int companyId)
    {
        const string sql = @"
            SELECT COUNT(*) 
            FROM Employees 
            WHERE CompanyId = @CompanyId";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int)
                    { Value = companyId });

                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }
    }
}