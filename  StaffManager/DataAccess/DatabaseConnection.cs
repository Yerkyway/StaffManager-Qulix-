using System.Data.SqlClient;

namespace StaffManager.DataAccess;

public class DatabaseConnection
{
    private readonly string _connectionString;
    
    public DatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public SqlConnection getConnection()
    {
        return new SqlConnection(_connectionString);
    }
}