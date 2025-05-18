using Bookify.Domain.Abstractions;
using Bookify.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bookify.Application.IntegrationTests.Infrastructure;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly AppDbContext AppDbContext;
    protected readonly IUnitOfWork UnitOfWork;

    protected BaseIntegrationTest(IntegrationTestWebFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        UnitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }
}
