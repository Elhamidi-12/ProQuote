using Microsoft.EntityFrameworkCore;

using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Supplier-specific data access operations.
/// </summary>
public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="SupplierRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SupplierRepository(AppDbContext context) : base(context)
    {
    }

    #endregion

    #region Query Methods

    /// <inheritdoc />
    public async Task<Supplier?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Categories)
                .ThenInclude(sc => sc.Category)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Supplier?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);

        return await DbSet
            .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Supplier?> GetWithCategoriesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Categories)
                .ThenInclude(sc => sc.Category)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Supplier>> GetByStatusAsync(
        SupplierStatus status,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Categories)
                .ThenInclude(sc => sc.Category)
            .Where(s => s.Status == status)
            .OrderBy(s => s.CompanyName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Supplier>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Categories)
                .ThenInclude(sc => sc.Category)
            .Where(s => s.Status == SupplierStatus.Approved &&
                        s.Categories.Any(sc => sc.CategoryId == categoryId))
            .OrderBy(s => s.CompanyName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Supplier>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(searchTerm);

        string pattern = $"%{searchTerm}%";

        return await DbSet
            .Include(s => s.Categories)
                .ThenInclude(sc => sc.Category)
            .Where(s => s.Status == SupplierStatus.Approved &&
                        (EF.Functions.Like(s.CompanyName, pattern) ||
                         EF.Functions.Like(s.ContactName, pattern) ||
                         EF.Functions.Like(s.Email, pattern)))
            .OrderBy(s => s.CompanyName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Supplier>> GetTopRatedAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.Status == SupplierStatus.Approved && s.TotalRatings > 0)
            .OrderByDescending(s => s.AverageRating)
            .ThenByDescending(s => s.TotalRatings)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    #endregion
}
