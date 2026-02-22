using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

    public static IDbContextFactory<AppDbContext> CreateFactory(string databaseName)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        using AppDbContext context = new(options);
        context.Database.EnsureCreated();

        return new InMemoryAppDbContextFactory(options);
    }

    private sealed class InMemoryAppDbContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new AppDbContext(options));
    }
}
