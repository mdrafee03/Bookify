using Bookify.Domain.Abstractions;

namespace Bookify.Domain.UnitTests.Infrastructure;

public abstract class BaseTest
{
    public static T AssertDomainEventWasPublished<T>(Entity entity)
    {
        var domainEvents = entity.GetDomainEvents().OfType<T>().SingleOrDefault();

        if (domainEvents is null)
        {
            throw new Exception($"Domain event of type {typeof(T).Name} was not published.");
        }

        return domainEvents;
    }
}
