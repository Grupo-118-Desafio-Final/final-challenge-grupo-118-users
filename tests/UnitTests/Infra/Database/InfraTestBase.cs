using Infra.Database.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace UnitTests.Infra.Database;

public abstract class InfraTestBase : IDisposable
{
    protected readonly AppDbContext DbContext;

    protected InfraTestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        DbContext = new AppDbContext(options);
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}
