using ProQuote.Domain.Entities;
using ProQuote.Infrastructure.Identity;
using ProQuote.Infrastructure.Services;
using ProQuote.Infrastructure.Tests.TestHelpers;
using Xunit;

namespace ProQuote.Infrastructure.Tests;

public class CommunicationServiceTests
{
    [Fact]
    public async Task MarkAllNotificationsAsReadAsync_ShouldMarkUnreadNotifications()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        ApplicationUserIdentity user = new()
        {
            Id = Guid.NewGuid(),
            UserName = "buyer@test.local",
            Email = "buyer@test.local",
            FirstName = "Buyer",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(user);
        await context.Notifications.AddRangeAsync(
        [
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Title = "N1",
                Message = "Unread 1",
                Type = "Test",
                IsRead = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Title = "N2",
                Message = "Unread 2",
                Type = "Test",
                IsRead = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Title = "N3",
                Message = "Already read",
                Type = "Test",
                IsRead = true,
                ReadAt = DateTime.UtcNow,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            }
        ]);
        await context.SaveChangesAsync();

        CommunicationService service = new(context);
        int updated = await service.MarkAllNotificationsAsReadAsync(user.Id);
        int unreadCount = await service.GetUnreadNotificationCountAsync(user.Id);

        Assert.Equal(2, updated);
        Assert.Equal(0, unreadCount);
    }
}
