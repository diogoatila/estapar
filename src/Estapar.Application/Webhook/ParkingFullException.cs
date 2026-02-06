namespace Estapar.Application.Webhook;

public sealed class ParkingFullException : Exception
{
    public ParkingFullException(string message) : base(message)
    {
    }
}