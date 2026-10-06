using CommissionManager.Domain.Common;

namespace CommissionManager.Domain.Errors;

public static class SaleErrors
{
    public static readonly Error EmptySellerId =
        Error.Validation("Sale.EmptySellerId", "O Id do vendedor é obrigatorio");
    
    public static readonly Error InvalidValue = 
        Error.Validation("Sale.InvalidValue", "O valor da venda deve ser maior que zero");
}
