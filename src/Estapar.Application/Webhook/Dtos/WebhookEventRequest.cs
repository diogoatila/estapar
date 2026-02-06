using System.Text.Json.Serialization;

namespace Estapar.Application.Webhook.Dtos;

public sealed class WebhookEventRequest
{
    [JsonPropertyName("license_plate")]
    public string LicensePlate { get; set; } = default!;

    [JsonPropertyName("entry_time")]
    public DateTime? EntryTime { get; set; }

    [JsonPropertyName("exit_time")]
    public DateTime? ExitTime { get; set; }

    [JsonPropertyName("lat")]
    public decimal? Lat { get; set; }

    [JsonPropertyName("lng")]
    public decimal? Lng { get; set; }

    [JsonPropertyName("event_type")]
    public string EventType { get; set; } = default!;
}