using Bookify.Domain.Apartments;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal sealed class ApartmentRepository(AppDbContext dbContext)
    : Repository<Apartment>(dbContext),
        IApartmentRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task UpdateAsync(
        Apartment apartment,
        CancellationToken cancellationToken = default
    )
    {
        // await _dbContext.Apartments.ExecuteUpdateAsync(, cancellationToken);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(
        Apartment apartment,
        CancellationToken cancellationToken = default
    )
    {
        await Task.CompletedTask;
        // await _dbContext.Apartments.ExecuteDeleteAsync( cancellationToken: cancellationToken);
    }
}
