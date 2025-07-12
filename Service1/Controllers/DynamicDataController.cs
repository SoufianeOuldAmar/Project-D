using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/data")]
public class DynamicDataController : ControllerBase
{
    private readonly DynamicDataService _service;

    public DynamicDataController(IConfiguration configuration)
    {
        _service = new DynamicDataService(configuration);
    }

    [HttpGet("{tableName}")]
    public IActionResult GetTableData(string tableName)
    {
        try
        {
            var filters = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

            var dataTable = _service.GetTableData(tableName, filters);
            var result = new List<Dictionary<string, object>>();

            foreach (DataRow row in dataTable.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                result.Add(dict);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
