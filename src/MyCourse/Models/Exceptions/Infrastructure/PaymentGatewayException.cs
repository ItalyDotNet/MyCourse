namespace MyCourse.Models.Exceptions.Infrastructure;

public class PaymentGatewayException : Exception
{
    public PaymentGatewayException(Exception innerException) : base($"Payment gateway threw an exception", innerException)
    {
    }
}
