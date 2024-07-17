using System.Data;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;


namespace HotelAppLibrary.Databases;

public class SqlDataAccess : IDataAccess
{
    private readonly IConfiguration _config;
    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }
    
    
    public List<T> LoadData<T, U>(string sql,
        U parameters,
        string connectionStringName,
        dynamic options = null)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);
        CommandType? commandType = CommandType.Text;
        
        if(options.IsStoredProcedure != null && options.IsStoredProcedure == true)
        {
            commandType = CommandType.StoredProcedure;
        }
        
        
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            List<T> rows = connection.Query<T>(sql, parameters, commandType: commandType).ToList();
            return rows;
        }
    }
    
    public void SaveData<T>(string sql,
        T parameters,
        string connectionStringName,
        dynamic options = null)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);
        CommandType? commandType = CommandType.Text;
        
        if(options.IsStoredProcedure != null && options.IsStoredProcedure == true)
        {
            commandType = CommandType.StoredProcedure;
        }
        
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            connection.Execute(sql, parameters, commandType: commandType);
        }
    }
    
}