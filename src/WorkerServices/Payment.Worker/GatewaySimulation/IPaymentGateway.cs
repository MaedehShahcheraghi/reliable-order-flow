namespace Payment.Worker.GatewaySimulation;

public interface IPaymentGateway
{
    Task<PaymentGatewayResult> ChargeAsync(
        Guid orderId,
        decimal amount,
        CancellationToken cancellationToken);
}
public sealed record PaymentGatewayResult(
    bool IsSuccessful,
    string? FailureReason);