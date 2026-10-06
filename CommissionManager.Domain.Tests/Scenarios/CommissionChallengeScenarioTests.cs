using CommissionManager.Domain.Entities;

namespace CommissionManager.Domain.Tests.Scenarios;

public class CommissionChallengeScenarioTests
{
    private static readonly (string Seller, decimal Value)[] ChallengeSales =
    [
        ("João Silva", 1200.50m), ("João Silva", 950.75m), ("João Silva", 1800.00m),
        ("João Silva", 1400.30m), ("João Silva", 1100.90m), ("João Silva", 1550.00m),
        ("João Silva", 1700.80m), ("João Silva", 250.30m), ("João Silva", 480.75m),
        ("João Silva", 320.40m),
        ("Maria Souza", 2100.40m), ("Maria Souza", 1350.60m), ("Maria Souza", 950.20m),
        ("Maria Souza", 1600.75m), ("Maria Souza", 1750.00m), ("Maria Souza", 1450.90m),
        ("Maria Souza", 400.50m), ("Maria Souza", 180.20m), ("Maria Souza", 90.75m),
        ("Carlos Oliveira", 800.50m), ("Carlos Oliveira", 1200.00m), ("Carlos Oliveira", 1950.30m),
        ("Carlos Oliveira", 1750.80m), ("Carlos Oliveira", 1300.60m), ("Carlos Oliveira", 300.40m),
        ("Carlos Oliveira", 500.00m), ("Carlos Oliveira", 125.75m),
        ("Ana Lima", 1000.00m), ("Ana Lima", 1100.50m), ("Ana Lima", 1250.75m),
        ("Ana Lima", 1400.20m), ("Ana Lima", 1550.90m), ("Ana Lima", 1650.00m),
        ("Ana Lima", 75.30m), ("Ana Lima", 420.90m), ("Ana Lima", 315.40m),
    ];

    private static Dictionary<string, Seller> BuildSellers()
    {
        var sellers = new Dictionary<string, Seller>();

        foreach (var (name, value) in ChallengeSales)
        {
            if (!sellers.TryGetValue(name, out var seller))
            {
                var sellerResult = Seller.Create(name);
                Assert.True(sellerResult.IsSuccess);
                seller = sellerResult.Value;
                sellers[name] = seller;
            }

            var saleResult = Sale.Create(seller.SellerId, value);
            Assert.True(saleResult.IsSuccess);
            Assert.True(seller.AddSale(saleResult.Value).IsSuccess);
        }

        return sellers;
    }

    public static TheoryData<string, int, decimal, decimal> ExpectedResults => new()
    {
        { "João Silva", 10, 10754.70m, 495.69m },
        { "Maria Souza", 9, 9874.30m, 465.96m },
        { "Carlos Oliveira", 8, 7928.35m, 379.38m },
        { "Ana Lima", 9, 8763.95m, 404.99m },
    };

    [Theory]
    [MemberData(nameof(ExpectedResults))]
    public void ChallengeData_ProducesExpectedTotalsPerSeller(
        string name, int expectedCount, decimal expectedTotalSales, decimal expectedCommission)
    {
        var seller = BuildSellers()[name];

        Assert.Equal(expectedCount, seller.Sales.Count);
        Assert.Equal(expectedTotalSales, seller.TotalSales);
        Assert.Equal(expectedCommission, seller.TotalCommission);
    }

    [Fact]
    public void ChallengeData_CreatesFourSellersWithAllSales()
    {
        var sellers = BuildSellers();

        Assert.Equal(4, sellers.Count);
        Assert.Equal(ChallengeSales.Length, sellers.Values.Sum(s => s.Sales.Count));
        Assert.Equal(1746.02m, sellers.Values.Sum(s => s.TotalCommission));
    }

    [Fact]
    public void ChallengeData_EachSellerHasUniqueId()
    {
        var sellers = BuildSellers();

        Assert.Equal(sellers.Count, sellers.Values.Select(s => s.SellerId).Distinct().Count());
    }
}
