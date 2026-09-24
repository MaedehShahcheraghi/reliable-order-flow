namespace Order.Domain.Entities.Eums;

public enum OrderStatus
{
    Submitted = 1,

    Processing = 2,

    Paid = 3,

    Completed = 4,

    Cancelled = 5,

    failed=6
}
