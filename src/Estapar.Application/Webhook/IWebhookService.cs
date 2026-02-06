using Estapar.Application.Webhook.Dtos;

namespace Estapar.Application.Webhook;
public interface IWebhookService
{
    Task ProcessAsync(WebhookEventRequest request, CancellationToken ct);
}