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

    /// <summary>
    /// Retorna a configuração da garagem (setores + vagas).
    /// </summary>
    /// <remarks>
    /// Este endpoint existe para cumprir o contrato do simulador do teste.
    /// 
    /// Exemplo de resposta:
    /// 
    /// {
    ///   "garage": [
    ///     {
    ///       "sector": "A",
    ///       "basePrice": 10.0,
    ///       "max_capacity": 5
    ///     }
    ///   ],
    ///   "spots": [
    ///     {
    ///       "id": 1,
    ///       "sector": "A",
    ///       "lat": -23.561684,
    ///       "lng": -46.655981
    ///     }
    ///   ]
    /// }
    /// </remarks>
    /// <response code="200">Configuração retornada com sucesso.</response>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _service.GetGarageAsync(ct);
        return Ok(result);
    }
}