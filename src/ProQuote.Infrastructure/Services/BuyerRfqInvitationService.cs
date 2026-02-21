using Microsoft.EntityFrameworkCore;

using ProQuote.Application.DTOs.Invitations;
using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Services;

/// <summary>
/// Service implementation for buyer RFQ supplier invitations.
/// </summary>
public class BuyerRfqInvitationService : IBuyerRfqInvitationService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuyerRfqInvitationService"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="auditLogService">Audit log service.</param>
    public BuyerRfqInvitationService(AppDbContext context, IAuditLogService auditLogService)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(auditLogService);
        _context = context;
        _auditLogService = auditLogService;
    }

    /// <inheritdoc />
    public async Task<BuyerRfqInvitationContextDto?> GetInvitationContextAsync(Guid buyerUserId, Guid rfqId)
    {
        Rfq? rfq = await _context.Rfqs
            .Include(r => r.Category)
            .Include(r => r.Invitations)
            .FirstOrDefaultAsync(r => r.Id == rfqId && r.BuyerId == buyerUserId);

        if (rfq == null)
        {
            return null;
        }

        List<Supplier> approvedSuppliers = await _context.Suppliers
            .Include(s => s.Categories)
            .Where(s => s.Status == SupplierStatus.Approved)
            .OrderBy(s => s.CompanyName)
            .ToListAsync();

        Dictionary<Guid, RfqInvitation> invitationMap = rfq.Invitations.ToDictionary(i => i.SupplierId);
        BuyerRfqInvitationContextDto context = new()
        {
            RfqId = rfq.Id,
            ReferenceNumber = rfq.ReferenceNumber,
            Title = rfq.Title,
            Status = rfq.Status,
            CategoryId = rfq.CategoryId,
            CategoryName = rfq.Category.Name,
            SubmissionDeadline = rfq.SubmissionDeadline,
            Candidates = approvedSuppliers.Select(s => new BuyerInvitationSupplierCandidateDto
            {
                SupplierId = s.Id,
                CompanyName = s.CompanyName,
                ContactName = s.ContactName,
                Email = s.Email,
                CategoryMatch = s.Categories.Any(c => c.CategoryId == rfq.CategoryId),
                AlreadyInvited = invitationMap.ContainsKey(s.Id),
                InvitationStatus = invitationMap.TryGetValue(s.Id, out RfqInvitation? inv) ? inv.Status : null
            }).ToList()
        };

        return context;
    }

    /// <inheritdoc />
    public async Task<SendRfqInvitationsResponse> SendInvitationsAsync(Guid buyerUserId, Guid rfqId, IReadOnlyCollection<Guid> supplierIds)
    {
        if (supplierIds == null || supplierIds.Count == 0)
        {
            return SendRfqInvitationsResponse.Failure("Select at least one supplier.");
        }

        Rfq? rfq = await _context.Rfqs
            .Include(r => r.Invitations)
            .FirstOrDefaultAsync(r => r.Id == rfqId && r.BuyerId == buyerUserId);

        if (rfq == null)
        {
            return SendRfqInvitationsResponse.Failure("RFQ not found.");
        }

        if (rfq.Status != RfqStatus.Published && rfq.Status != RfqStatus.QuotesReceived)
        {
            return SendRfqInvitationsResponse.Failure("RFQ must be published before sending invitations.");
        }

        if (rfq.SubmissionDeadline <= DateTime.UtcNow)
        {
            return SendRfqInvitationsResponse.Failure("RFQ submission deadline has passed.");
        }

        HashSet<Guid> distinctSupplierIds = supplierIds.Where(id => id != Guid.Empty).ToHashSet();
        if (distinctSupplierIds.Count == 0)
        {
            return SendRfqInvitationsResponse.Failure("No valid suppliers selected.");
        }

        List<Supplier> suppliers = await _context.Suppliers
            .Where(s => distinctSupplierIds.Contains(s.Id) && s.Status == SupplierStatus.Approved)
            .ToListAsync();

        HashSet<Guid> existingInvitationSupplierIds = rfq.Invitations.Select(i => i.SupplierId).ToHashSet();
        DateTime now = DateTime.UtcNow;
        int sent = 0;
        int skipped = 0;

        foreach (Supplier supplier in suppliers)
        {
            if (existingInvitationSupplierIds.Contains(supplier.Id))
            {
                skipped++;
                continue;
            }

            RfqInvitation invitation = new()
            {
                Id = Guid.NewGuid(),
                RfqId = rfq.Id,
                SupplierId = supplier.Id,
                SecureToken = Guid.NewGuid().ToString("N"),
                Status = InvitationStatus.Sent,
                SentAt = now,
                TokenExpiresAt = rfq.SubmissionDeadline,
                CreatedAt = now
            };

            await _context.RfqInvitations.AddAsync(invitation);
            await _context.Notifications.AddAsync(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = supplier.UserId,
                Title = "New RFQ Invitation",
                Message = $"You were invited to quote on {rfq.ReferenceNumber}.",
                Type = "RfqInvitation",
                ActionUrl = "/supplier/invitations",
                RelatedEntityId = rfq.Id,
                RelatedEntityType = "Rfq",
                IsRead = false,
                SentAt = now,
                CreatedAt = now
            });

            existingInvitationSupplierIds.Add(supplier.Id);
            sent++;
        }

        skipped += distinctSupplierIds.Count - suppliers.Count;

        await _context.SaveChangesAsync();

        if (sent > 0)
        {
            await _auditLogService.LogRfqActionAsync(
                rfq.Id,
                buyerUserId,
                "RfqInvitationsSent",
                "RfqInvitation",
                entityId: null,
                details: $"Sent {sent} invitation(s), skipped {skipped}.");
        }

        return SendRfqInvitationsResponse.Success(sent, skipped, $"Invitations sent: {sent}, skipped: {skipped}.");
    }
}
