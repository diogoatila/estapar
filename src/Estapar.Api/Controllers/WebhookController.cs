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