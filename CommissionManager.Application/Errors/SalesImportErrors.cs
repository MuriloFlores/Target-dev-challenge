using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Errors;

public sealed class SalesImportErrors
{
    public static readonly Error SourceNotFound =
        Error.NotFound("SalesImport.SourceNotFound", "A fonte de vendas não foi encontrada.");

    public static readonly Error InvalidFormat =
        Error.Validation("SalesImport.InvalidFormat", "O conteúdo das vendas está em um formato inválido.");

    public static readonly Error Empty =
        Error.Validation("SalesImport.Empty", "Nenhuma venda foi informada.");
}