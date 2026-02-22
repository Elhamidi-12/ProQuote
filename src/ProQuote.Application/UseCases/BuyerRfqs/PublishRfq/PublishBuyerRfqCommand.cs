namespace ProQuote.Application.UseCases.BuyerRfqs.PublishRfq;

/// <summary>
/// Command payload for publishing an RFQ.
/// </summary>
/// <param name="BuyerUserId">Current buyer user identifier.</param>
/// <param name="RfqId">RFQ identifier.</param>
/// <param name="RemoteIpAddress">Optional remote IP address for audit logging.</param>
public sealed record PublishBuyerRfqCommand(
    Guid BuyerUserId,
    Guid RfqId,
    string? RemoteIpAddress = null);
