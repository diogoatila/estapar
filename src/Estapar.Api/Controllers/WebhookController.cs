using Estapar.Application.Webhook;
using Estapar.Application.Webhook.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Estapar.Api.Controllers;

[ApiController]
[Route("webhook")]
public sealed class WebhookController : ControllerBase
{
    private readonly IWebhookService _service;

    public WebhookController(IWebhookService service)
    {
        _service = service;
    }

    /// <summary>
    /// Recebe eventos do simulador (ENTRY, PARKED, EXIT) e atualiza o estado do estacionamento.
    /// </summary>
    /// <remarks>
    /// Tipos de eventos aceitos:
    ///
    /// ENTRY:
    /// {
    ///   "license_plate": "ZUL0001",
    ///   "entry_time": "2026-01-01T12:00:00.000Z",
    ///   "event_type": "ENTRY"
    /// }
    ///
    /// PARKED:
    /// {
    ///   "license_plate": "ZUL0001",
    ///   "lat": -23.561684,
    ///   "lng": -46.655981,
    ///   "event_type": "PARKED"
    /// }
    ///
    /// EXIT:
    /// {
    ///   "license_plate": "ZUL0001",
    ///   "exit_time": "2026-01-01T13:40:00.000Z",
    ///   "event_type": "EXIT"
    /// }
    ///
    /// Regras:
    /// - ENTRY ocupa uma vaga e cria uma sessão.
    /// - PARKED marca a sessão como estacionada.
    /// - EXIT libera a vaga e calcula o valor.
    /// </remarks>
    /// <param name="request">Evento recebido do simulador.</param>
    /// <param name="ct">CancellationToken</param>
    /// <response code="200">Evento processado com sucesso (idempotente).</response>
    /// <response code="400">Payload inválido (campos obrigatórios ausentes).</response>
    /// <response code="409">Estacionamento cheio (ENTRY bloqueado).</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] WebhookEventRequest request, CancellationToken ct)
    {
        await _service.ProcessAsync(request, ct);
        return Ok();
    }
}