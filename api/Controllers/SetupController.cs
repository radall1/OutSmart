using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class SetupController(IConfiguration config) : ControllerBase {
    private readonly IConfiguration _config = config;

    [HttpPost("initialize")]
    public IActionResult RunSqlScript() {
        var sqlPath = Path.Combine(Directory.GetCurrentDirectory(), "SqlScripts", "0001.sql");
        if (!System.IO.File.Exists(sqlPath))
            return NotFound("SQL script not found.");

        var script = System.IO.File.ReadAllText(sqlPath);
        var connStr = _config.GetConnectionString("Default");

        using var conn = new SqlConnection(connStr);
        conn.Open();
        using var cmd = new SqlCommand(script, conn);
        cmd.ExecuteNonQuery();

        return Ok("Database initialized.");
    }
}
