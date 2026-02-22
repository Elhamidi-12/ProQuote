using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProQuote.Application.DTOs.Communication;
using ProQuote.Application.Interfaces;
using ProQuote.Application.UseCases.Notifications.MarkAllNotificationsAsRead;
using ProQuote.Application.UseCases.Notifications.MarkNotificationAsRead;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Notification endpoints for authenticated users.
/// </summary>
[Route("api/v1/notifications")]
[Authorize(Roles = ApplicationRoles.All, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationsController : ApiControllerBase
{
    private readonly ICommunicationService _communicationService;
    private readonly IMarkNotificationAsReadUseCase _markNotificationAsReadUseCase;
    private readonly IMarkAllNotificationsAsReadUseCase _markAllNotificationsAsReadUseCase;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsController"/> class.
    /// </summary>
    /// <param name="communicationService">Communication service.</param>
    /// <param name="markNotificationAsReadUseCase">Mark single notification as read use-case.</param>
    /// <param name="markAllNotificationsAsReadUseCase">Mark all notifications as read use-case.</param>
    public NotificationsController(
        ICommunicationService communicationService,
        IMarkNotificationAsReadUseCase markNotificationAsReadUseCase,
        IMarkAllNotificationsAsReadUseCase markAllNotificationsAsReadUseCase)
    {
        _communicationService = communicationService;
        _markNotificationAsReadUseCase = markNotificationAsReadUseCase;
        _markAllNotificationsAsReadUseCase = markAllNotificationsAsReadUseCase;
    }

    /// <summary>
    /// Gets notifications for current user.
    /// </summary>
    /// <param name="take">Max item count.</param>
    /// <param name="unreadOnly">Only unread notifications.</param>
    /// <returns>Notification list payload.</returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int take = 20, [FromQuery] bool unreadOnly = false)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        IReadOnlyList<NotificationItemDto> notifications =
            await _communicationService.GetNotificationsAsync(CurrentUserId.Value, take, unreadOnly);
        int unreadCount = await _communicationService.GetUnreadNotificationCountAsync(CurrentUserId.Value);

        return Ok(new
        {
            unreadCount,
            items = notifications
        });
    }

    /// <summary>
    /// Marks one notification as read.
    /// </summary>
    /// <param name="id">Notification identifier.</param>
    /// <returns>Operation result.</returns>
    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        MarkNotificationAsReadResponse response = await _markNotificationAsReadUseCase.ExecuteAsync(
            new MarkNotificationAsReadCommand(CurrentUserId.Value, id));

        return response.Succeeded
            ? Ok(new { succeeded = true })
            : NotFound(new { succeeded = false });
    }

    /// <summary>
    /// Marks all notifications as read.
    /// </summary>
    /// <returns>Operation result.</returns>
    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        MarkAllNotificationsAsReadResponse response = await _markAllNotificationsAsReadUseCase.ExecuteAsync(
            new MarkAllNotificationsAsReadCommand(CurrentUserId.Value));

        return Ok(new { succeeded = response.Succeeded, updated = response.UpdatedCount });
    }
}
