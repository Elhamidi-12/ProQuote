namespace ProQuote.Application.UseCases.BuyerRfqs.Invitations;

/// <summary>
/// Command payload for sending supplier invitations for a buyer RFQ.
/// </summary>
/// <param name="BuyerUserId">Current buyer user identifier.</param>
/// <param name="RfqId">RFQ identifier.</param>
/// <param name="SupplierIds">Supplier identifiers to invite.</param>
public sealed record SendRfqInvitationsCommand(
    Guid BuyerUserId,
    Guid RfqId,
    IReadOnlyCollection<Guid> SupplierIds);
