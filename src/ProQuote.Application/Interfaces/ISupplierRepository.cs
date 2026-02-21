using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Repository interface for Supplier-specific data access operations.
/// </summary>
public interface ISupplierRepository : IRepository<Supplier>
{
    #region Query Methods

    /// <summary>
    /// Gets a supplier by their user identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The supplier if found; otherwise, null.</returns>
    Task<Supplier?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a supplier by their email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The supplier if found; otherwise, null.</returns>
    Task<Supplier?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a supplier with all their categories.
    /// </summary>
    /// <param name="id">The supplier identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The supplier with categories if found; otherwise, null.</returns>
    Task<Supplier?> GetWithCategoriesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all suppliers in a specific status.
    /// </summary>
    /// <param name="status">The supplier status.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of suppliers in the specified status.</returns>
    Task<IReadOnlyList<Supplier>> GetByStatusAsync(SupplierStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all approved suppliers in a specific category.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of approved suppliers in the category.</returns>
    Task<IReadOnlyList<Supplier>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for suppliers by company name or contact name.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of matching suppliers.</returns>
    Task<IReadOnlyList<Supplier>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top-rated suppliers.
    /// </summary>
    /// <param name="count">The number of suppliers to return.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of top-rated suppliers.</returns>
    Task<IReadOnlyList<Supplier>> GetTopRatedAsync(int count, CancellationToken cancellationToken = default);

    #endregion
}
