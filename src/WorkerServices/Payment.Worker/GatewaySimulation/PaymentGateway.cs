namespace Payment.Worker.GatewaySimulation;

public class PaymentGateway : IPaymentGateway
{
    public Task<PaymentGatewayResult> ChargeAsync(Guid orderId, decimal amount, CancellationToken cancellationToken)
    {
    if (amount == 400)
        {
            return Task.FromResult(
                new PaymentGatewayResult(
                    IsSuccessful: false,
                    FailureReason: "Card declined"));
        }


        if (amount == 503)
        {
            throw new TimeoutException(
                "Payment gateway timed out.");
        }


        return Task.FromResult(
            new PaymentGatewayResult(
                IsSuccessful: true,
                FailureReason: null));
    }
}