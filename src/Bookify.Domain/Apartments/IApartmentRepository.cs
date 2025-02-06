namespace Bookify.Domain.Apartments;

public interface IApartmentRepository
{
    Task<Apartment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Apartment apartment);
    Task UpdateAsync(Apartment apartment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Apartment apartment, CancellationToken cancellationToken = default);
}
