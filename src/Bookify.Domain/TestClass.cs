using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Domain;

public class TestClass
{
    public void GetUser()
    {
        var name = new Apartments.Name("John Doe");
        var description = new Apartments.Description("This is a description");
        var user = User.CreateUser(
            new FirstName("Md"),
            new LastName("Rafee"),
            new Email("mdfdfsdsfsa@sfs.com")
        );
        var result = Result.Success(user);
        // result.Value
    }
}
