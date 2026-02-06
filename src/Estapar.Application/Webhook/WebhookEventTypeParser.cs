namespace Estapar.Application.Webhook;

public static class WebhookEventTypeParser
{
    public static WebhookEventType Parse(string value)
    {
        if (Enum.TryParse<WebhookEventType>(value?.Trim(), ignoreCase: true, out var parsed))
            return parsed;

        throw new ArgumentException($"Invalid event_type: '{value}'");
    }
}