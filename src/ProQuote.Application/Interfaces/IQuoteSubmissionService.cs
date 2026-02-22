using ProQuote.Application.DTOs.Quotes;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Service contract for supplier invitation and quote submission flows.
/// </summary>
public interface IQuoteSubmissionService
{
    /// <summary>
    /// Gets supplier invitations for the current supplier user.
    /// </summary>
    /// <param name="supplierUserId">Supplier user identifier.</param>
    /// <param name="status">Optional invitation status filter.</param>
    /// <returns>A collection of invitation items.</returns>
    public Task<IReadOnlyList<SupplierInvitationItemDto>> GetSupplierInvitationsAsync(
        Guid supplierUserId,
        InvitationStatus? status = null);

    /// <summary>
    /// Gets supplier quotes for the current supplier user.
    /// </summary>
    /// <param name="supplierUserId">Supplier user identifier.</param>
    /// <param name="status">Optional quote status filter.</param>
    /// <returns>A collection of supplier quote items.</returns>
    public Task<IReadOnlyList<SupplierQuoteListItemDto>> GetSupplierQuotesAsync(
        Guid supplierUserId,
        QuoteStatus? status = null);

    /// <summary>
    /// Gets quote editor payload for RFQ and current supplier.
    /// </summary>
    /// <param name="supplierUserId">Supplier user identifier.</param>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <returns>Quote editor payload, or null if not accessible.</returns>
    public Task<QuoteEditorDto?> GetQuoteEditorAsync(Guid supplierUserId, Guid rfqId);

    /// <summary>
    /// Saves quote for RFQ and supplier.
    /// </summary>
    /// <param name="supplierUserId">Supplier user identifier.</param>
    /// <param name="request">Quote save request.</param>
    /// <returns>Save response.</returns>
    public Task<QuoteSaveResponse> SaveQuoteAsync(Guid supplierUserId, SaveQuoteRequest request);

    /// <summary>
    /// Recalculates and persists quality score snapshot for an existing supplier quote.
    /// </summary>
    /// <param name="supplierUserId">Supplier user identifier.</param>
    /// <param name="quoteId">Quote identifier.</param>
    /// <param name="eventType">Optional event type label for timeline history.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RecalculateQuoteQualitySnapshotAsync(Guid supplierUserId, Guid quoteId, string? eventType = null);
}
