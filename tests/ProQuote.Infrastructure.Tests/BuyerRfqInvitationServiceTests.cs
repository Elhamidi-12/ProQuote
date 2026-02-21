using Microsoft.EntityFrameworkCore;

using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Identity;
using ProQuote.Infrastructure.Services;
using ProQuote.Infrastructure.Tests.TestHelpers;
using Xunit;

namespace ProQuote.Infrastructure.Tests;

public class BuyerRfqInvitationServiceTests
{
    [Fact]
    public async Task SendInvitationsAsync_ShouldCreateInvitations_AndNotifications()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        (ApplicationUserIdentity buyer, Rfq rfq, Supplier supplierA, Supplier supplierB) = await SeedScenarioAsync(context);
        AuditLogService auditLogService = new(context);
        BuyerRfqInvitationService service = new(context, auditLogService);

        var response = await service.SendInvitationsAsync(buyer.Id, rfq.Id, [supplierA.Id, supplierB.Id]);

        int invitationCount = await context.RfqInvitations.CountAsync(i => i.RfqId == rfq.Id);
        int notificationCount = await context.Notifications.CountAsync(n => n.RelatedEntityId == rfq.Id && n.Type == "RfqInvitation");
        AuditLog? audit = await context.AuditLogs.FirstOrDefaultAsync(a => a.RfqId == rfq.Id && a.Action == "RfqInvitationsSent");

        Assert.True(response.Succeeded);
        Assert.Equal(2, response.SentCount);
        Assert.Equal(0, response.SkippedCount);
        Assert.Equal(2, invitationCount);
        Assert.Equal(2, notificationCount);
        Assert.NotNull(audit);
    }

    [Fact]
    public async Task SendInvitationsAsync_ShouldSkipAlreadyInvitedSuppliers()
    {
        using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString());

        (ApplicationUserIdentity buyer, Rfq rfq, Supplier supplierA, Supplier supplierB) = await SeedScenarioAsync(context);
        await context.RfqInvitations.AddAsync(new RfqInvitation
        {
            Id = Guid.NewGuid(),
            RfqId = rfq.Id,
            SupplierId = supplierA.Id,
            SecureToken = Guid.NewGuid().ToString("N"),
            Status = InvitationStatus.Sent,
            SentAt = DateTime.UtcNow,
            TokenExpiresAt = rfq.SubmissionDeadline,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        AuditLogService auditLogService = new(context);
        BuyerRfqInvitationService service = new(context, auditLogService);

        var response = await service.SendInvitationsAsync(buyer.Id, rfq.Id, [supplierA.Id, supplierB.Id]);
        int invitationCount = await context.RfqInvitations.CountAsync(i => i.RfqId == rfq.Id);

        Assert.True(response.Succeeded);
        Assert.Equal(1, response.SentCount);
        Assert.Equal(1, response.SkippedCount);
        Assert.Equal(2, invitationCount);
    }

    private static async Task<(ApplicationUserIdentity Buyer, Rfq Rfq, Supplier SupplierA, Supplier SupplierB)> SeedScenarioAsync(
        Infrastructure.Data.AppDbContext context)
    {
        ApplicationUserIdentity buyer = new()
        {
            Id = Guid.NewGuid(),
            UserName = "buyer-invite@test.local",
            Email = "buyer-invite@test.local",
            FirstName = "Buyer",
            LastName = "Invite",
            CreatedAt = DateTime.UtcNow
        };

        ApplicationUserIdentity supplierUserA = new()
        {
            Id = Guid.NewGuid(),
            UserName = "supplier-invite-a@test.local",
            Email = "supplier-invite-a@test.local",
            FirstName = "Supplier",
            LastName = "InviteA",
            CreatedAt = DateTime.UtcNow
        };

        ApplicationUserIdentity supplierUserB = new()
        {
            Id = Guid.NewGuid(),
            UserName = "supplier-invite-b@test.local",
            Email = "supplier-invite-b@test.local",
            FirstName = "Supplier",
            LastName = "InviteB",
            CreatedAt = DateTime.UtcNow
        };

        Category category = new()
        {
            Id = Guid.NewGuid(),
            Name = "IT Services",
            Description = "IT Services",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        Supplier supplierA = new()
        {
            Id = Guid.NewGuid(),
            UserId = supplierUserA.Id,
            CompanyName = "Alpha Systems",
            ContactName = "Alice",
            Email = supplierUserA.Email!,
            Status = SupplierStatus.Approved,
            RegisteredAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        Supplier supplierB = new()
        {
            Id = Guid.NewGuid(),
            UserId = supplierUserB.Id,
            CompanyName = "Beta Tech",
            ContactName = "Bob",
            Email = supplierUserB.Email!,
            Status = SupplierStatus.Approved,
            RegisteredAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        Rfq rfq = new()
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = $"RFQ-{DateTime.UtcNow.Year}-77777",
            Title = "Invitation Flow Test",
            Description = "Invitation test RFQ",
            CategoryId = category.Id,
            Currency = "USD",
            SubmissionDeadline = DateTime.UtcNow.AddDays(5),
            Status = RfqStatus.Published,
            BuyerId = buyer.Id,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddRangeAsync(buyer, supplierUserA, supplierUserB);
        await context.Categories.AddAsync(category);
        await context.Suppliers.AddRangeAsync(supplierA, supplierB);
        await context.Rfqs.AddAsync(rfq);
        await context.SaveChangesAsync();

        return (buyer, rfq, supplierA, supplierB);
    }
}
