using CommissionManager.Domain.Common;

namespace CommissionManager.Domain.Errors;

public static class SellerErrors
{
    public static readonly Error EmptyName =
        Error.Validation("Seller.EmptyName", "O nome do vendedor é obrigatório");

    public static readonly Error SaleFromAnotherSeller =
        Error.Validation("Seller.SaleFromAnotherSeller", "A venda pertence a outro vendedor");

    public static readonly Error DuplicateSale =
        Error.Conflict("Seller.DuplicateSale", "Esta venda já foi adiciona ao vendedor");
}
