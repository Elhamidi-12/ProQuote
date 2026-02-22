using Microsoft.EntityFrameworkCore;

using ProQuote.Application.DTOs.Communication;
using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Services;

/// <summary>
/// In-app communication service for notifications and RFQ messages.
/// </summary>
public class CommunicationService : ICommunicationService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommunicationService"/> class.
    /// </summary>
    /// <param name="dbContextFactory">Database context factory.</param>
    public CommunicationService(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        ArgumentNullException.ThrowIfNull(dbContextFactory);
        _dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationItemDto>> GetNotificationsAsync(Guid userId, int take = 20, bool unreadOnly = false)
    {
        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        IQueryable<Notification> query = context.Notifications
            .Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.SentAt)
            .Take(take)
            .Select(n => new NotificationItemDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                ActionUrl = n.ActionUrl,
                IsRead = n.IsRead,
                SentAt = n.SentAt
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
    {
        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    /// <inheritdoc />
    public async Task<bool> MarkNotificationAsReadAsync(Guid userId, Guid notificationId)
    {
        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Notification? notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == notificationId);

        if (notification == null)
        {
            return false;
        }

        notification.MarkAsRead();
        context.Notifications.Update(notification);
        await context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<int> MarkAllNotificationsAsReadAsync(Guid userId)
    {
        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        List<Notification> notifications = await context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (Notification notification in notifications)
        {
            notification.MarkAsRead();
        }

        if (notifications.Count > 0)
        {
            context.Notifications.UpdateRange(notifications);
            await context.SaveChangesAsync();
        }

        return notifications.Count;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MessagingRfqOptionDto>> GetBuyerRfqOptionsAsync(Guid buyerUserId)
    {
        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        return await context.Rfqs
            .Where(r => r.BuyerId == buyerUserId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new MessagingRfqOptionDto
            {
                RfqId = r.Id,
                ReferenceNumber = r.ReferenceNumber,
                Title = r.Title
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MessagingRfqOptionDto>> GetSupplierRfqOptionsAsync(Guid supplierUserId)
    {
        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Supplier? supplier = await context.Suppliers
            .FirstOrDefaultAsync(s => s.UserId == supplierUserId);

        if (supplier == null)
        {
            return [];
        }

        return await context.RfqInvitations
            .Where(i => i.SupplierId == supplier.Id)
            .Select(i => i.Rfq)
            .Distinct()
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new MessagingRfqOptionDto
            {
                RfqId = r.Id,
                ReferenceNumber = r.ReferenceNumber,
                Title = r.Title
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MessageThreadItemDto>> GetBuyerMessagesAsync(Guid buyerUserId, Guid? rfqId = null, int take = 100)
    {
        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        IQueryable<QaMessage> query = context.QaMessages
            .Where(m => m.Rfq.BuyerId == buyerUserId);

        if (rfqId.HasValue)
        {
            query = query.Where(m => m.RfqId == rfqId.Value);
        }

        return await query
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .Select(m => new MessageThreadItemDto
            {
                Id = m.Id,
                RfqId = m.RfqId,
                RfqReferenceNumber = m.Rfq.ReferenceNumber,
                RfqTitle = m.Rfq.Title,
                SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                Message = m.Message,
                IsFromBuyer = m.IsFromBuyer,
                IsPrivate = m.TargetSupplierId.HasValue,
                SentAt = m.SentAt
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MessageThreadItemDto>> GetSupplierMessagesAsync(Guid supplierUserId, Guid? rfqId = null, int take = 100)
    {
        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Supplier? supplier = await context.Suppliers
            .FirstOrDefaultAsync(s => s.UserId == supplierUserId);

        if (supplier == null)
        {
            return [];
        }

        IQueryable<QaMessage> query = context.QaMessages
            .Where(m =>
                m.Rfq.Invitations.Any(i => i.SupplierId == supplier.Id) &&
                (!m.TargetSupplierId.HasValue || m.TargetSupplierId == supplier.Id));

        if (rfqId.HasValue)
        {
            query = query.Where(m => m.RfqId == rfqId.Value);
        }

        return await query
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .Select(m => new MessageThreadItemDto
            {
                Id = m.Id,
                RfqId = m.RfqId,
                RfqReferenceNumber = m.Rfq.ReferenceNumber,
                RfqTitle = m.Rfq.Title,
                SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                Message = m.Message,
                IsFromBuyer = m.IsFromBuyer,
                IsPrivate = m.TargetSupplierId.HasValue,
                SentAt = m.SentAt
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> SendBuyerMessageAsync(Guid buyerUserId, Guid rfqId, string message, Guid? targetSupplierId = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Rfq? rfq = await context.Rfqs
            .Include(r => r.Invitations)
            .FirstOrDefaultAsync(r => r.Id == rfqId && r.BuyerId == buyerUserId);

        if (rfq == null)
        {
            return false;
        }

        if (targetSupplierId.HasValue && !rfq.Invitations.Any(i => i.SupplierId == targetSupplierId.Value))
        {
            return false;
        }

        QaMessage qaMessage = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfqId,
            SenderId = buyerUserId,
            TargetSupplierId = targetSupplierId,
            Message = message.Trim(),
            SentAt = DateTime.UtcNow,
            IsFromBuyer = true,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await context.QaMessages.AddAsync(qaMessage);

        IEnumerable<Guid> targetSupplierIds = targetSupplierId.HasValue
            ? [targetSupplierId.Value]
            : rfq.Invitations.Select(i => i.SupplierId).Distinct();

        List<Guid> recipientUserIds = await context.Suppliers
            .Where(s => targetSupplierIds.Contains(s.Id))
            .Select(s => s.UserId)
            .Distinct()
            .ToListAsync();

        foreach (Guid userId in recipientUserIds)
        {
            await context.Notifications.AddAsync(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "New Buyer Message",
                Message = $"New message on {rfq.ReferenceNumber}.",
                Type = "QaMessage",
                ActionUrl = "/supplier/messages",
                RelatedEntityId = rfqId,
                RelatedEntityType = "Rfq",
                IsRead = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> SendSupplierMessageAsync(Guid supplierUserId, Guid rfqId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        await using AppDbContext context = await _dbContextFactory.CreateDbContextAsync();

        Supplier? supplier = await context.Suppliers
            .FirstOrDefaultAsync(s => s.UserId == supplierUserId);

        if (supplier == null)
        {
            return false;
        }

        Rfq? rfq = await context.Rfqs
            .Include(r => r.Invitations)
            .FirstOrDefaultAsync(r => r.Id == rfqId);

        if (rfq == null || !rfq.Invitations.Any(i => i.SupplierId == supplier.Id))
        {
            return false;
        }

        QaMessage qaMessage = new()
        {
            Id = Guid.NewGuid(),
            RfqId = rfqId,
            SenderId = supplierUserId,
            TargetSupplierId = null,
            Message = message.Trim(),
            SentAt = DateTime.UtcNow,
            IsFromBuyer = false,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await context.QaMessages.AddAsync(qaMessage);
        await context.Notifications.AddAsync(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = rfq.BuyerId,
            Title = "New Supplier Message",
            Message = $"New message on {rfq.ReferenceNumber}.",
            Type = "QaMessage",
            ActionUrl = "/buyer/messages",
            RelatedEntityId = rfqId,
            RelatedEntityType = "Rfq",
            IsRead = false,
            SentAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
        return true;
    }
}
