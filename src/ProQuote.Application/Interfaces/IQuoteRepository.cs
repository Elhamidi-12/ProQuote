using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Repository interface for Quote-specific data access operations.
/// </summary>
public interface IQuoteRepository : IRepository<Quote>
{
    #region Query Methods

    /// <summary>
    /// Gets a quote with all its related data (line items, documents).
    /// </summary>
    /// <param name="id">The quote identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quote with all related data if found; otherwise, null.</returns>
    public Task<Quote?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all quotes for a specific RFQ.
    /// </summary>
    /// <param name="rfqId">The RFQ identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of quotes for the RFQ.</returns>
    public Task<IReadOnlyList<Quote>> GetByRfqIdAsync(Guid rfqId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all quotes submitted by a specific supplier.
    /// </summary>
    /// <param name="supplierId">The supplier identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of quotes submitted by the supplier.</returns>
    public Task<IReadOnlyList<Quote>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the quote submitted by a specific supplier for a specific RFQ.
    /// </summary>
    /// <param name="rfqId">The RFQ identifier.</param>
    /// <param name="supplierId">The supplier identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quote if found; otherwise, null.</returns>
    public Task<Quote?> GetByRfqAndSupplierAsync(Guid rfqId, Guid supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all quotes in a specific status.
    /// </summary>
    /// <param name="status">The quote status.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of quotes in the specified status.</returns>
    public Task<IReadOnlyList<Quote>> GetByStatusAsync(QuoteStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the awarded quote for a specific RFQ.
    /// </summary>
    /// <param name="rfqId">The RFQ identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The awarded quote if found; otherwise, null.</returns>
    public Task<Quote?> GetAwardedQuoteForRfqAsync(Guid rfqId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all quotes for an RFQ with line items for comparison.
    /// </summary>
    /// <param name="rfqId">The RFQ identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of quotes with line items.</returns>
    public Task<IReadOnlyList<Quote>> GetQuotesForComparisonAsync(Guid rfqId, CancellationToken cancellationToken = default);

    #endregion
}
