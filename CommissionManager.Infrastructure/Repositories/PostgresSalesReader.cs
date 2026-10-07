using CommissionManager.Application.Dtos;
using CommissionManager.Application.Interfaces;
using CommissionManager.Domain.Common;
using CommissionManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CommissionManager.Infrastructure.Repositories;

internal sealed class PostgresSalesReader(AppDbContext context) : ISalesReader
{
    public async Task<Result<IReadOnlyList<SaleInputDto>>> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        var sales = await context.Sales
            .OrderBy(s => s.Id)
            .Select(s => new SaleInputDto(s.SellerName, s.Value))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<SaleInputDto>>.Success(sales);
    }
}