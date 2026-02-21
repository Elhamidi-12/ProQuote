using ProQuote.Application.DTOs.Invitations;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Service contract for buyer RFQ supplier invitation workflows.
/// </summary>
public interface IBuyerRfqInvitationService
{
    /// <summary>
    /// Gets RFQ invitation context and supplier candidates for buyer.
    /// </summary>
    /// <param name="buyerUserId">Buyer user identifier.</param>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <returns>Invitation context, or null if inaccessible.</returns>
    public Task<BuyerRfqInvitationContextDto?> GetInvitationContextAsync(Guid buyerUserId, Guid rfqId);

    /// <summary>
    /// Sends invitations to selected suppliers for an RFQ.
    /// </summary>
    /// <param name="buyerUserId">Buyer user identifier.</param>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="supplierIds">Supplier ids to invite.</param>
    /// <returns>Operation result.</returns>
    public Task<SendRfqInvitationsResponse> SendInvitationsAsync(Guid buyerUserId, Guid rfqId, IReadOnlyCollection<Guid> supplierIds);
}
