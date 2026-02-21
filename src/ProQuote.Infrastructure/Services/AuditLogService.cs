using Microsoft.EntityFrameworkCore;

using ProQuote.Application.DTOs.Audit;
using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Services;

/// <summary>
/// Service implementation for RFQ audit logs.
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLogService"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public AuditLogService(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <inheritdoc />
    public async Task LogRfqActionAsync(
        Guid rfqId,
        Guid userId,
        string action,
        string entityType,
        Guid? entityId = null,
        string? oldValue = null,
        string? newValue = null,
        string? details = null,
        string? ipAddress = null)
    {
        if (rfqId == Guid.Empty || userId == Guid.Empty)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(action) || string.IsNullOrWhiteSpace(entityType))
        {
            return;
        }

        AuditLog entry = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfqId,
            UserId = userId,
            Action = action.Trim(),
            EntityType = entityType.Trim(),
            EntityId = entityId,
            OldValue = oldValue,
            NewValue = newValue,
            Details = details,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _context.AuditLogs.AddAsync(entry);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLogListItemDto>> GetRecentRfqAuditLogsAsync(
        int take = 200,
        Guid? rfqId = null,
        string? action = null)
    {
        int maxTake = Math.Clamp(take, 1, 1000);
        IQueryable<AuditLog> query = _context.AuditLogs
            .Include(a => a.Rfq)
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp);

        if (rfqId.HasValue)
        {
            query = query.Where(a => a.RfqId == rfqId.Value);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            string trimmed = action.Trim();
            query = query.Where(a => a.Action == trimmed);
        }

        return await query
            .Take(maxTake)
            .Select(a => new AuditLogListItemDto
            {
                Id = a.Id,
                RfqId = a.RfqId,
                RfqReferenceNumber = a.Rfq.ReferenceNumber,
                UserId = a.UserId,
                UserDisplayName = string.IsNullOrWhiteSpace(a.User.FirstName) && string.IsNullOrWhiteSpace(a.User.LastName)
                    ? a.User.Email ?? a.UserId.ToString()
                    : $"{a.User.FirstName} {a.User.LastName}".Trim(),
                Action = a.Action,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Details = a.Details,
                Timestamp = a.Timestamp
            })
            .ToListAsync();
    }
}
