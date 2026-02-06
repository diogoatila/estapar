using Estapar.Application.Revenue;
using Microsoft.AspNetCore.Mvc;

namespace Estapar.Api.Controllers;

[ApiController]
[Route("revenue")]
public sealed class RevenueController : ControllerBase
{
    private readonly IRevenueService _service;

    public RevenueController(IRevenueService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] string date, [FromQuery] string sector, CancellationToken ct)
    {
        if (!DateOnly.TryParse(date, out var parsedDate))
            return BadRequest(new { error = "Invalid date format. Use yyyy-MM-dd." });

        var result = await _service.GetRevenueAsync(parsedDate, sector, ct);
        return Ok(result);
    }
}