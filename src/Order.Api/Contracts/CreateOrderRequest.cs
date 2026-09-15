namespace Order.Api.Contracts;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    Guid ProductId,
    int Quantity,
    decimal TotalAmount);