using RFQApp.Domain.Entities;
using RFQApp.Domain.Enums;

namespace RFQApp.Application.Interfaces;

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
    Task<Rfq?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an RFQ with all its related data (line items, invitations, quotes, etc.).
    /// </summary>
    /// <param name="id">The RFQ identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The RFQ with all related data if found; otherwise, null.</returns>
    Task<Rfq?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs created by a specific buyer.
    /// </summary>
    /// <param name="buyerId">The buyer identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs created by the buyer.</returns>
    Task<IReadOnlyList<Rfq>> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs in a specific status.
    /// </summary>
    /// <param name="status">The RFQ status.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs in the specified status.</returns>
    Task<IReadOnlyList<Rfq>> GetByStatusAsync(RfqStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs that a supplier has been invited to.
    /// </summary>
    /// <param name="supplierId">The supplier identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs the supplier has been invited to.</returns>
    Task<IReadOnlyList<Rfq>> GetBySupplierInvitationAsync(Guid supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all RFQs with submission deadlines within a specified period.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of RFQs within the deadline period.</returns>
    Task<IReadOnlyList<Rfq>> GetByDeadlinePeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets RFQs that have expired (deadline passed with no quotes).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of expired RFQs.</returns>
    Task<IReadOnlyList<Rfq>> GetExpiredRfqsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Reference Number Generation

    /// <summary>
    /// Generates the next RFQ reference number.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The next reference number in format RFQ-YYYY-NNNNN.</returns>
    Task<string> GenerateReferenceNumberAsync(CancellationToken cancellationToken = default);

    #endregion
}
