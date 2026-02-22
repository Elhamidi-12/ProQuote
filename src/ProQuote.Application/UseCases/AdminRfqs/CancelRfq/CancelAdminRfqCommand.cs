namespace ProQuote.Application.UseCases.AdminRfqs.CancelRfq;

/// <summary>
/// Command payload for cancelling an RFQ as admin.
/// </summary>
/// <param name="AdminUserId">Current admin user identifier.</param>
/// <param name="RfqId">RFQ identifier.</param>
/// <param name="Reason">Optional cancellation reason.</param>
/// <param name="RemoteIpAddress">Optional remote IP address.</param>
public sealed record CancelAdminRfqCommand(
    Guid AdminUserId,
    Guid RfqId,
    string? Reason = null,
    string? RemoteIpAddress = null);
