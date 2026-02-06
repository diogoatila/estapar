namespace Estapar.Application.Revenue.Dtos;
public sealed class RevenueResponseDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BRL";
    public DateTime Timestamp { get; set; }
}