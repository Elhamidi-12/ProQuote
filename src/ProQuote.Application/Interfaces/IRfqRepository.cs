using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Repository interface for RFQ-specific data access operations.
/// </summary>
public interface IRfqRepository : IRepository<Rfq>
{
    #region Query Methods

    /// <summary>
    /// Gets an RFQ by its reference number.
    /// </summary>
    /// <param name="referenceNumber">The RFQ reference number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The RFQ if found; otherwise, null.</returns>
    public Task<Rfq?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an RFQ with all its related data (line items, invitations, quotes, etc.).
    /// </summary>
    /// <param name="id">The RFQ identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The RFQ with all related data if found; otherwise, null.</returns>
    public Task<Rfq?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs created by a specific buyer.
    /// </summary>
    /// <param name="buyerId">The buyer identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs created by the buyer.</returns>
    public Task<IReadOnlyList<Rfq>> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs created by a specific buyer with related details used by reporting.
    /// </summary>
    /// <param name="buyerId">The buyer identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs with related details.</returns>
    public Task<IReadOnlyList<Rfq>> GetByBuyerIdWithDetailsAsync(Guid buyerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs in a specific status.
    /// </summary>
    /// <param name="status">The RFQ status.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs in the specified status.</returns>
    public Task<IReadOnlyList<Rfq>> GetByStatusAsync(RfqStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs that a supplier has been invited to.
    /// </summary>
    /// <param name="supplierId">The supplier identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs the supplier has been invited to.</returns>
    public Task<IReadOnlyList<Rfq>> GetBySupplierInvitationAsync(Guid supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs with submission deadlines within a specified period.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs within the deadline period.</returns>
    public Task<IReadOnlyList<Rfq>> GetByDeadlinePeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets RFQs that have expired (deadline passed with no quotes).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of expired RFQs.</returns>
    public Task<IReadOnlyList<Rfq>> GetExpiredRfqsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs with related details used by admin dashboards and reports.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs with related details.</returns>
    public Task<IReadOnlyList<Rfq>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets RFQ documents ordered by display order.
    /// </summary>
    /// <param name="rfqId">The RFQ identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQ documents.</returns>
    public Task<IReadOnlyList<RfqDocument>> GetDocumentsAsync(Guid rfqId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets RFQ document by identifier.
    /// </summary>
    /// <param name="documentId">The document identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The RFQ document if found; otherwise, null.</returns>
    public Task<RfqDocument?> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the supplier has an invitation for the RFQ.
    /// </summary>
    /// <param name="rfqId">The RFQ identifier.</param>
    /// <param name="supplierId">The supplier identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when invitation exists; otherwise, false.</returns>
    public Task<bool> HasSupplierInvitationAsync(Guid rfqId, Guid supplierId, CancellationToken cancellationToken = default);

    #endregion

    #region Reference Number Generation

    /// <summary>
    /// Generates the next RFQ reference number.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The next reference number in format RFQ-YYYY-NNNNN.</returns>
    public Task<string> GenerateReferenceNumberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a collection of RFQ documents.
    /// </summary>
    /// <param name="documents">The RFQ documents.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task AddDocumentsAsync(IEnumerable<RfqDocument> documents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an RFQ document.
    /// </summary>
    /// <param name="document">The document to remove.</param>
    public void RemoveDocument(RfqDocument document);

    #endregion
}
