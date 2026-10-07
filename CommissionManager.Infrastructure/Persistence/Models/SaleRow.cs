namespace CommissionManager.Infrastructure.Persistence.Models;

internal sealed class SaleRow
{
    public Guid Id { get; init; }
    public string SellerName { get; init; } = string.Empty;
    public decimal Value { get; init; }
}
