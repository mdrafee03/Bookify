namespace Bookify.Infrastructure.Repositories;

public sealed class EntityNotFoundException<T> : Exception
{
    public EntityNotFoundException()
        : base($"Entity of type {typeof(T).Name} was not found.") { }

    public EntityNotFoundException(string message)
        : base(message) { }
}
