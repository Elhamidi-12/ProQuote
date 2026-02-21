namespace ProQuote.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing database transactions.
/// </summary>
/// <remarks>
/// The Unit of Work pattern coordinates the work of multiple repositories
/// and ensures all changes are committed as a single transaction.
/// </remarks>
public interface IUnitOfWork : IDisposable
{
    #region Repositories

    /// <summary>
    /// Gets the RFQ repository.
    /// </summary>
    public IRfqRepository Rfqs { get; }

    /// <summary>
    /// Gets the Quote repository.
    /// </summary>
    public IQuoteRepository Quotes { get; }

    /// <summary>
    /// Gets the Supplier repository.
    /// </summary>
    public ISupplierRepository Suppliers { get; }

    /// <summary>
    /// Gets the Category repository.
    /// </summary>
    public ICategoryRepository Categories { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities written to the database.</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    #endregion
}
