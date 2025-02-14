using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public interface IApartmentRepository : IRepository<Apartment>
{
    Task UpdateAsync(Apartment apartment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Apartment apartment, CancellationToken cancellationToken = default);
}
