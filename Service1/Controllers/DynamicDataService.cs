using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

public class DynamicDataService
{
    private readonly IConfiguration _configuration;

    public DynamicDataService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DataTable GetTableData(string tableName, Dictionary<string, string>? filters = null)
    {
        var connectionString = _configuration.GetConnectionString("FlightExportDb");
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var sql = $"SELECT * FROM [{tableName}]";
        var parameters = new List<SqlParameter>();

        if (filters != null && filters.Any())
        {
            var whereClauses = filters.Select((kv, i) =>
            {
                var paramName = $"@param{i}";
                parameters.Add(new SqlParameter(paramName, kv.Value));
                return $"[{kv.Key}] = {paramName}";
            });
            sql += " WHERE " + string.Join(" AND ", whereClauses);
        }

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters.ToArray());

        using var adapter = new SqlDataAdapter(command);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }
}
