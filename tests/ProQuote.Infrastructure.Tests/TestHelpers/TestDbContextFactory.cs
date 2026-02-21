using Microsoft.EntityFrameworkCore;

using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Tests.TestHelpers;

internal static class TestDbContextFactory
{
    public static AppDbContext Create(string databaseName)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        AppDbContext context = new(options);
        context.Database.EnsureCreated();
        return context;
    }
}
