namespace Bookify.Application.Abstractions.Authentication;

public interface IUserContext
{
    public Guid UserId { get; }
    public string IdentityId { get; }
}
