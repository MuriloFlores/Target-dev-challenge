using CommissionManager.Domain.Entities;
using CommissionManager.Domain.Errors;

namespace CommissionManager.Domain.Tests.Entities;

public class SellerTests
{
    private static Seller NewSeller(string name = "João Silva") => Seller.Create(name).Value;

    private static Sale NewSale(Seller seller, decimal value) => Sale.Create(seller.SellerId, value).Value;

    [Fact]
    public void Create_WithValidName_ReturnsSellerWithoutSales()
    {
        var result = Seller.Create("João Silva");

        Assert.True(result.IsSuccess);
        Assert.Equal("João Silva", result.Value.Name);
        Assert.NotEqual(Guid.Empty, result.Value.SellerId);
        Assert.Empty(result.Value.Sales);
        Assert.Equal(0m, result.Value.TotalSales);
        Assert.Equal(0m, result.Value.TotalCommission);
    }

    [Fact]
    public void Create_TrimsName()
    {
        var seller = NewSeller("  João Silva  ");

        Assert.Equal("João Silva", seller.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ReturnsEmptyNameError(string? name)
    {
        var result = Seller.Create(name!);

        Assert.True(result.IsFailure);
        Assert.Equal(SellerErrors.EmptyName, result.Error);
    }

    [Fact]
    public void AddSale_FromSameSeller_AddsSale()
    {
        var seller = NewSeller();
        var sale = NewSale(seller, 1200.50m);

        var result = seller.AddSale(sale);

        Assert.True(result.IsSuccess);
        Assert.Single(seller.Sales);
        Assert.Same(sale, seller.Sales[0]);
    }

    [Fact]
    public void AddSale_FromAnotherSeller_ReturnsErrorAndDoesNotAdd()
    {
        var seller = NewSeller("João Silva");
        var otherSeller = NewSeller("Maria Souza");
        var sale = NewSale(otherSeller, 1200.50m);

        var result = seller.AddSale(sale);

        Assert.True(result.IsFailure);
        Assert.Equal(SellerErrors.SaleFromAnotherSeller, result.Error);
        Assert.Empty(seller.Sales);
    }

    [Fact]
    public void AddSale_SameSaleTwice_ReturnsDuplicateErrorAndKeepsOne()
    {
        var seller = NewSeller();
        var sale = NewSale(seller, 1200.50m);
        seller.AddSale(sale);

        var result = seller.AddSale(sale);

        Assert.True(result.IsFailure);
        Assert.Equal(SellerErrors.DuplicateSale, result.Error);
        Assert.Single(seller.Sales);
    }

    [Fact]
    public void AddSale_SameValueDifferentSales_AddsBoth()
    {
        var seller = NewSeller();

        seller.AddSale(NewSale(seller, 100m));
        seller.AddSale(NewSale(seller, 100m));

        Assert.Equal(2, seller.Sales.Count);
    }

    [Fact]
    public void TotalSales_SumsAllSaleValues()
    {
        var seller = NewSeller();
        seller.AddSale(NewSale(seller, 90.75m));
        seller.AddSale(NewSale(seller, 400.50m));
        seller.AddSale(NewSale(seller, 2100.40m));

        Assert.Equal(2591.65m, seller.TotalSales);
    }

    [Fact]
    public void TotalCommission_SumsCommissionOfEachSale()
    {
        var seller = NewSeller();
        seller.AddSale(NewSale(seller, 90.75m));    // 0%  -> 0,00
        seller.AddSale(NewSale(seller, 400.50m));   // 1%  -> 4,005 -> 4,01
        seller.AddSale(NewSale(seller, 2100.40m));  // 5%  -> 105,02

        Assert.Equal(109.03m, seller.TotalCommission);
    }

    [Fact]
    public void Sales_CannotBeModifiedFromOutside()
    {
        var seller = NewSeller();
        seller.AddSale(NewSale(seller, 100m));

        var sales = Assert.IsAssignableFrom<ICollection<Sale>>(seller.Sales);

        Assert.True(sales.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => sales.Add(NewSale(seller, 200m)));
    }
}
