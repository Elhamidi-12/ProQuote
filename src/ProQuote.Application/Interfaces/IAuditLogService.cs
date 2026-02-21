using ProQuote.Application.DTOs.Audit;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Service contract for RFQ audit logs.
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Writes a new RFQ-related audit entry.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <param name="userId">Actor user identifier.</param>
    /// <param name="action">Action label.</param>
    /// <param name="entityType">Affected entity type.</param>
    /// <param name="entityId">Affected entity identifier.</param>
    /// <param name="oldValue">Optional old value JSON/text.</param>
    /// <param name="newValue">Optional new value JSON/text.</param>
    /// <param name="details">Optional details text.</param>
    /// <param name="ipAddress">Optional remote IP.</param>
    /// <returns>Task.</returns>
    public Task LogRfqActionAsync(
        Guid rfqId,
        Guid userId,
        string action,
        string entityType,
        Guid? entityId = null,
        string? oldValue = null,
        string? newValue = null,
        string? details = null,
        string? ipAddress = null);

    /// <summary>
    /// Gets recent RFQ audit logs.
    /// </summary>
    /// <param name="take">Max number of records.</param>
    /// <param name="rfqId">Optional RFQ filter.</param>
    /// <param name="action">Optional action filter.</param>
    /// <returns>Audit list.</returns>
    public Task<IReadOnlyList<AuditLogListItemDto>> GetRecentRfqAuditLogsAsync(
        int take = 200,
        Guid? rfqId = null,
        string? action = null);
}
