namespace ProQuote.Application.UseCases.AdminRfqs.MoveToEvaluation;

/// <summary>
/// Command payload for moving an RFQ to under evaluation as admin.
/// </summary>
/// <param name="AdminUserId">Current admin user identifier.</param>
/// <param name="RfqId">RFQ identifier.</param>
/// <param name="RemoteIpAddress">Optional remote IP address.</param>
public sealed record MoveAdminRfqToEvaluationCommand(
    Guid AdminUserId,
    Guid RfqId,
    string? RemoteIpAddress = null);
