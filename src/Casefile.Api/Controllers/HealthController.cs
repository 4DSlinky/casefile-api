using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Casefile.Api.Data;

namespace Casefile.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var canQuery = await db.Database.CanConnectAsync(ct);
        return Ok(new
        {
            status = canQuery ? "ok" : "degraded",
            utc = DateTimeOffset.UtcNow
        });
    }
}
