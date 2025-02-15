namespace Bookify.Domain.Abstractions;

public abstract class Entity(Guid id)
{
    private readonly List<IDomainEvent> _domainEvents = [];

    private Entity()
        : this(Guid.Empty) { }

    public Guid Id { get; init; } = id;

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();

    public void ClearDomainEvents() => _domainEvents.Clear();
}
