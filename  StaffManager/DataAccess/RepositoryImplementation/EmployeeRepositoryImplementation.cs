using System.Data;
using System.Data.SqlClient;
using StaffManager.Models;
using StaffManager.Repository;


namespace StaffManager.DataAccess.RepositoryImplementation;

public class EmployeeRepositoryImplementation : IEmployeeRepository
{

    /// <summary>
    /// defines a connection to the database.
    /// </summary>

    private readonly DatabaseConnection _databaseConnection;

    public EmployeeRepositoryImplementation(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }


    /// <summary>
    /// defines a method to retrieve all employees from the database.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<EmployeeModel>> GetAllEmployeesAsync()
    {
        var employees = new List<EmployeeModel>();

        const string sql = @"
    SELECT e.Id, e.FirstName, e.MiddleName, e.LastName, 
           e.Position, e.HireDate, e.CompanyId, c.Name as CompanyName, c.LegalForm
    FROM Employees e
    LEFT JOIN Companies c ON e.CompanyId = c.Id
    ORDER BY e.LastName";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new EmployeeModel
                        {
                            Id = reader.GetInt32("Id"),
                            FirstName = reader.GetString("FirstName"),
                            MiddleName = reader.IsDBNull("MiddleName") ? null : reader.GetString("MiddleName"),
                            LastName = reader.GetString("LastName"),
                            Position = (Position)Enum.Parse(typeof(Position), reader.GetString("Position")),
                            HireDate = reader.GetDateTime("HireDate"),
                            CompanyId = reader.GetInt32("CompanyId"),
                            Company = new CompanyModel
                            {
                                Id = reader.GetInt32("CompanyId"),
                                Name = reader.GetString("CompanyName"),
                                LegalForm = reader.GetString("LegalForm")
                            }
                        });
                    }
                }
            }
        }

        return employees;
    }


    /// <summary>
    /// defines a method to retrieve an employee by their ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<EmployeeModel> GetEmployeeByIdAsync(int id)
    {
        EmployeeModel employee = null;

        const string sql = @"
    SELECT e.Id, e.FirstName, e.MiddleName, e.LastName, 
           e.Position, e.HireDate, e.CompanyId, c.Name as CompanyName, c.LegalForm
    FROM Employees e
    LEFT JOIN Companies c ON e.CompanyId = c.Id
    WHERE e.Id = @Id";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)
                    { Value = id });

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        employee = new EmployeeModel
                        {
                            Id = reader.GetInt32("Id"),
                            FirstName = reader.GetString("FirstName"),
                            MiddleName = reader.IsDBNull("MiddleName") ? null : reader.GetString("MiddleName"),
                            LastName = reader.GetString("LastName"),
                            Position = (Position)Enum.Parse(typeof(Position), reader.GetString("Position")),
                            HireDate = reader.GetDateTime("HireDate"),
                            CompanyId = reader.GetInt32("CompanyId"),
                            Company = new CompanyModel
                            {
                                Id = reader.GetInt32("CompanyId"),
                                Name = reader.GetString("CompanyName"),
                                LegalForm = reader.GetString("LegalForm")
                            }
                        };
                    }
                }
            }

            return employee;
        }

    }

    public async Task<int> CreateEmployeeAsync(EmployeeModel employee)
    {
        if (employee == null)
        {
            throw new ArgumentNullException(nameof(employee));
        }

        const string sql = @"
            INSERT INTO Employees (FirstName, MiddleName, LastName, Position, HireDate, CompanyId)
            VALUES (@FirstName, @MiddleName, @LastName, @Position, @HireDate, @CompanyId);
            SELECT SCOPE_IDENTITY();";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100)
                    { Value = employee.FirstName ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@MiddleName", SqlDbType.NVarChar, 100)
                    { Value = employee.MiddleName ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100)
                    { Value = employee.LastName ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Position", SqlDbType.NVarChar, 50)
                    { Value = employee.Position.ToString() });
                command.Parameters.Add(new SqlParameter("@HireDate", SqlDbType.DateTime)
                    { Value = employee.HireDate });
                command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int)
                    { Value = employee.CompanyId});
                
                
                Console.WriteLine($"Inserting Employee: {employee.FirstName} {employee.LastName}, Position={employee.Position}, CompanyId={employee.CompanyId}");
                var result = await command.ExecuteScalarAsync();
                Console.WriteLine($"Inserted ID: {result}");

                return Convert.ToInt32(result);
            }
        }
    }

    public async Task UpdateEmployeeAsync(EmployeeModel employee)
    {
        if (employee == null)
        {
            throw new ArgumentNullException(nameof(employee));
        }

        const string sql = @"
            UPDATE Employees
            SET FirstName = @FirstName, 
                MiddleName = @MiddleName, 
                LastName = @LastName, 
                Position = @Position, 
                HireDate = @HireDate, 
                CompanyId = @CompanyId
            WHERE Id = @Id";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("Id", SqlDbType.Int)
                    { Value = employee.Id });
                command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100)
                    { Value = employee.FirstName ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@MiddleName", SqlDbType.NVarChar, 100)
                    { Value = employee.MiddleName ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100)
                    { Value = employee.LastName ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Position", SqlDbType.NVarChar, 50)
                    { Value = employee.Position.ToString() });
                command.Parameters.Add(new SqlParameter("@HireDate", SqlDbType.DateTime)
                    { Value = employee.HireDate });
                command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int)
                    { Value = employee.CompanyId });

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        const string sql = @"
            DELETE FROM Employees
            WHERE Id = @Id";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)
                    { Value = id });

                await command.ExecuteNonQueryAsync();
            }
        }
    }
    
    
    /// <summary>
    /// defines a method to retrieve employees by their company ID.
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    public async Task<List<EmployeeModel>> GetEmployeesByCompanyIdAsync(int companyId)
    {
        var employees = new List<EmployeeModel>();

        const string sql = @"
            SELECT e.Id, e.FirstName, e.MiddleName, e.LastName, 
                   e.Position, e.HireDate, c.Id as CompanyId, c.Name as CompanyName
            FROM Employees e
            LEFT JOIN Companies c ON e.CompanyId = c.Id
            WHERE e.CompanyId = @CompanyId";

        using (var connection = _databaseConnection.getConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int)
                    { Value = companyId });

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new EmployeeModel
                        {
                            Id = reader.GetInt32("Id"),
                            FirstName = reader.GetString("FirstName"),
                            MiddleName = reader.IsDBNull("MiddleName") ? null : reader.GetString("MiddleName"),
                            LastName = reader.GetString("LastName"),
                            Position = (Position)Enum.Parse(typeof(Position), reader.GetString("Position")),
                            HireDate = reader.GetDateTime("HireDate"),
                            CompanyId = reader.GetInt32("CompanyId"),
                            Company = new CompanyModel
                            {
                                Id = reader.GetInt32("CompanyId"),
                                Name = reader.GetString("CompanyName"),
                                LegalForm = reader.GetString("LegalForm")
                            }
                        });
                    }
                }
            }
        }

        return employees;
    }
    
}
