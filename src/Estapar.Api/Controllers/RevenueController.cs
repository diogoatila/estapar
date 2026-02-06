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

    /// <summary>
    /// Retorna a receita total (somatório de AmountCharged) de um setor em uma data específica.
    /// </summary>
    /// <remarks>
    /// Considera apenas sessões finalizadas (EXIT).
    ///
    /// Exemplo:
    /// GET /revenue?date=2025-01-01&amp;sector=A
    /// </remarks>
    /// <param name="date">Data no formato YYYY-MM-DD</param>
    /// <param name="sector">Código do setor (ex: A, B, C)</param>
    /// <param name="ct">CancellationToken</param>
    /// <response code="200">Receita retornada com sucesso.</response>
    /// <response code="400">Parâmetros inválidos.</response>
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