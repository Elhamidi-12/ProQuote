namespace ProQuote.Application.UseCases.AdminRfqs.CloseRfq;

/// <summary>
/// Command payload for closing an RFQ as admin.
/// </summary>
/// <param name="AdminUserId">Current admin user identifier.</param>
/// <param name="RfqId">RFQ identifier.</param>
/// <param name="RemoteIpAddress">Optional remote IP address.</param>
public sealed record CloseAdminRfqCommand(
    Guid AdminUserId,
    Guid RfqId,
    string? RemoteIpAddress = null);
