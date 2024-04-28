using Microsoft.EntityFrameworkCore;

namespace UnitTests.Helpers;

public class MockDb : IDbContextFactory<EmployeeDatabase>
{
    public EmployeeDatabase CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<EmployeeDatabase>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new EmployeeDatabase(options);
    }
}