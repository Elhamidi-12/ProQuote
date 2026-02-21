using ProQuote.Application.DTOs.Communication;

namespace ProQuote.Application.Interfaces;

/// <summary>
/// Service contract for in-app messaging and notifications.
/// </summary>
public interface ICommunicationService
{
    /// <summary>
    /// Gets recent notifications for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="take">Maximum number of items.</param>
    /// <param name="unreadOnly">When true, returns only unread notifications.</param>
    /// <returns>A collection of notification items.</returns>
    public Task<IReadOnlyList<NotificationItemDto>> GetNotificationsAsync(Guid userId, int take = 20, bool unreadOnly = false);

    /// <summary>
    /// Gets unread notification count for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Unread count.</returns>
    public Task<int> GetUnreadNotificationCountAsync(Guid userId);

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="notificationId">The notification identifier.</param>
    /// <returns>True if notification was updated.</returns>
    public Task<bool> MarkNotificationAsReadAsync(Guid userId, Guid notificationId);

    /// <summary>
    /// Marks all notifications as read for a user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The number of notifications updated.</returns>
    public Task<int> MarkAllNotificationsAsReadAsync(Guid userId);

    /// <summary>
    /// Gets RFQs owned by buyer for message filtering/posting.
    /// </summary>
    /// <param name="buyerUserId">The buyer user identifier.</param>
    /// <returns>A collection of RFQ options.</returns>
    public Task<IReadOnlyList<MessagingRfqOptionDto>> GetBuyerRfqOptionsAsync(Guid buyerUserId);

    /// <summary>
    /// Gets invited RFQs for supplier for message filtering/posting.
    /// </summary>
    /// <param name="supplierUserId">The supplier user identifier.</param>
    /// <returns>A collection of RFQ options.</returns>
    public Task<IReadOnlyList<MessagingRfqOptionDto>> GetSupplierRfqOptionsAsync(Guid supplierUserId);

    /// <summary>
    /// Gets message thread items visible to a buyer user.
    /// </summary>
    /// <param name="buyerUserId">The buyer user identifier.</param>
    /// <param name="rfqId">Optional RFQ filter.</param>
    /// <param name="take">Maximum number of items.</param>
    /// <returns>A collection of thread items.</returns>
    public Task<IReadOnlyList<MessageThreadItemDto>> GetBuyerMessagesAsync(Guid buyerUserId, Guid? rfqId = null, int take = 100);

    /// <summary>
    /// Gets message thread items visible to a supplier user.
    /// </summary>
    /// <param name="supplierUserId">The supplier user identifier.</param>
    /// <param name="rfqId">Optional RFQ filter.</param>
    /// <param name="take">Maximum number of items.</param>
    /// <returns>A collection of thread items.</returns>
    public Task<IReadOnlyList<MessageThreadItemDto>> GetSupplierMessagesAsync(Guid supplierUserId, Guid? rfqId = null, int take = 100);

    /// <summary>
    /// Posts a message from buyer into an RFQ thread.
    /// </summary>
    /// <param name="buyerUserId">The buyer user identifier.</param>
    /// <param name="rfqId">The RFQ identifier.</param>
    /// <param name="message">Message body.</param>
    /// <param name="targetSupplierId">Optional private target supplier.</param>
    /// <returns>True when successfully created.</returns>
    public Task<bool> SendBuyerMessageAsync(Guid buyerUserId, Guid rfqId, string message, Guid? targetSupplierId = null);

    /// <summary>
    /// Posts a message from supplier into an RFQ thread.
    /// </summary>
    /// <param name="supplierUserId">The supplier user identifier.</param>
    /// <param name="rfqId">The RFQ identifier.</param>
    /// <param name="message">Message body.</param>
    /// <returns>True when successfully created.</returns>
    public Task<bool> SendSupplierMessageAsync(Guid supplierUserId, Guid rfqId, string message);
}
