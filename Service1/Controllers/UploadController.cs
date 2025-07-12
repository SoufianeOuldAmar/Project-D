using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public UploadController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("upload-csv")]
    public async Task<IActionResult> UploadCsv([FromQuery] string tableName, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Document needed.");

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>().ToList();

        if (records.Count == 0)
            return BadRequest("Empty data.");

        var columns = ((IDictionary<string, object>)records[0]).Keys.ToList();

        var connectionString = _configuration.GetConnectionString("FlightExportDb");
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var createTableCmd = $@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}')
        BEGIN
            CREATE TABLE [{tableName}] (
                {string.Join(",", columns.Select(c => $"[{c}] NVARCHAR(MAX)"))}
            )
        END";

        using var createCmd = new SqlCommand(createTableCmd, connection);
        await createCmd.ExecuteNonQueryAsync();

        foreach (var record in records)
        {
            var data = (IDictionary<string, object>)record;
            var insertCmd = $"INSERT INTO [{tableName}] ({string.Join(",", data.Keys.Select(k => $"[{k}]"))}) VALUES ({string.Join(",", data.Keys.Select(k => $"@{k}"))})";
            using var cmd = new SqlCommand(insertCmd, connection);
            foreach (var kv in data)
            {
                cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value ?? DBNull.Value);
            }
            await cmd.ExecuteNonQueryAsync();
        }

        return Ok(new { count = records.Count });
    }
}
