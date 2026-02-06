using Estapar.Application.Garage;
using Microsoft.AspNetCore.Mvc;

namespace Estapar.Api.Controllers;

[ApiController]
[Route("garage")]
public sealed class GarageController : ControllerBase
{
    private readonly IGarageQueryService _service;

    public GarageController(IGarageQueryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _service.GetGarageAsync(ct);
        return Ok(result);
    }
}