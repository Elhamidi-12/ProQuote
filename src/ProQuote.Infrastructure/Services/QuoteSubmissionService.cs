using Microsoft.EntityFrameworkCore;

using ProQuote.Application.DTOs.Quotes;
using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Services;

/// <summary>
/// Service implementation for supplier quote submission workflows.
/// </summary>
public class QuoteSubmissionService : IQuoteSubmissionService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteSubmissionService"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="auditLogService">Audit log service.</param>
    public QuoteSubmissionService(AppDbContext context, IAuditLogService auditLogService)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(auditLogService);
        _context = context;
        _auditLogService = auditLogService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SupplierInvitationItemDto>> GetSupplierInvitationsAsync(
        Guid supplierUserId,
        InvitationStatus? status = null)
    {
        Supplier? supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == supplierUserId);
        if (supplier == null)
        {
            return [];
        }

        IQueryable<RfqInvitation> query = _context.RfqInvitations
            .Where(i => i.SupplierId == supplier.Id)
            .Include(i => i.Rfq)
                .ThenInclude(r => r.Category);

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        return await query
            .OrderByDescending(i => i.SentAt)
            .Select(i => new SupplierInvitationItemDto
            {
                RfqId = i.RfqId,
                ReferenceNumber = i.Rfq.ReferenceNumber,
                Title = i.Rfq.Title,
                CategoryName = i.Rfq.Category.Name,
                Status = i.Status,
                SentAt = i.SentAt,
                SubmissionDeadline = i.Rfq.SubmissionDeadline,
                HasQuote = _context.Quotes.Any(q => q.RfqId == i.RfqId && q.SupplierId == supplier.Id),
                QuoteStatus = _context.Quotes
                    .Where(q => q.RfqId == i.RfqId && q.SupplierId == supplier.Id)
                    .Select(q => (QuoteStatus?)q.Status)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SupplierQuoteListItemDto>> GetSupplierQuotesAsync(Guid supplierUserId, QuoteStatus? status = null)
    {
        Supplier? supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == supplierUserId);
        if (supplier == null)
        {
            return [];
        }

        IQueryable<Quote> query = _context.Quotes
            .Where(q => q.SupplierId == supplier.Id)
            .Include(q => q.Rfq);

        if (status.HasValue)
        {
            query = query.Where(q => q.Status == status.Value);
        }

        return await query
            .OrderByDescending(q => q.SubmittedAt)
            .Select(q => new SupplierQuoteListItemDto
            {
                QuoteId = q.Id,
                RfqId = q.RfqId,
                ReferenceNumber = q.Rfq.ReferenceNumber,
                Title = q.Rfq.Title,
                Status = q.Status,
                TotalAmount = q.TotalAmount,
                SubmittedAt = q.SubmittedAt,
                ValidUntil = q.ValidUntil,
                IsAwarded = q.IsAwarded
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<QuoteEditorDto?> GetQuoteEditorAsync(Guid supplierUserId, Guid rfqId)
    {
        Supplier? supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == supplierUserId);
        if (supplier == null)
        {
            return null;
        }

        RfqInvitation? invitation = await _context.RfqInvitations
            .Include(i => i.Rfq)
                .ThenInclude(r => r.LineItems.OrderBy(li => li.DisplayOrder))
            .FirstOrDefaultAsync(i => i.RfqId == rfqId && i.SupplierId == supplier.Id);

        if (invitation == null)
        {
            return null;
        }

        Quote? quote = await _context.Quotes
            .Include(q => q.LineItems)
            .FirstOrDefaultAsync(q => q.RfqId == rfqId && q.SupplierId == supplier.Id);

        Dictionary<Guid, QuoteLineItem> quoteLineItemByLineItemId = quote?.LineItems.ToDictionary(li => li.LineItemId) ?? [];

        QuoteEditorDto editor = new()
        {
            RfqId = invitation.RfqId,
            ReferenceNumber = invitation.Rfq.ReferenceNumber,
            RfqTitle = invitation.Rfq.Title,
            Currency = invitation.Rfq.Currency,
            SubmissionDeadline = invitation.Rfq.SubmissionDeadline,
            QuoteId = quote?.Id,
            LeadTimeDays = quote?.LeadTimeDays ?? 14,
            ValidUntil = quote?.ValidUntil ?? DateTime.UtcNow.AddDays(30),
            PaymentTerms = quote?.PaymentTerms,
            Notes = quote?.Notes,
            LineItems = invitation.Rfq.LineItems
                .OrderBy(li => li.DisplayOrder)
                .Select(li => new QuoteEditorLineItemDto
                {
                    LineItemId = li.Id,
                    Name = li.Name,
                    Description = li.Description,
                    Quantity = li.Quantity,
                    UnitOfMeasure = li.UnitOfMeasure,
                    UnitPrice = quoteLineItemByLineItemId.TryGetValue(li.Id, out QuoteLineItem? qli) ? qli.UnitPrice : 0m,
                    Notes = quoteLineItemByLineItemId.TryGetValue(li.Id, out qli) ? qli.Notes : null
                })
                .ToList()
        };

        if (invitation.Status == InvitationStatus.Sent)
        {
            invitation.MarkAsViewed();
            _context.RfqInvitations.Update(invitation);
            await _context.SaveChangesAsync();
        }

        return editor;
    }

    /// <inheritdoc />
    public async Task<QuoteSaveResponse> SaveQuoteAsync(Guid supplierUserId, SaveQuoteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Supplier? supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == supplierUserId);
        if (supplier == null)
        {
            return QuoteSaveResponse.Failure("Supplier profile not found.");
        }

        RfqInvitation? invitation = await _context.RfqInvitations
            .Include(i => i.Rfq)
            .FirstOrDefaultAsync(i => i.RfqId == request.RfqId && i.SupplierId == supplier.Id);

        if (invitation == null)
        {
            return QuoteSaveResponse.Failure("Invitation not found.");
        }

        if (invitation.Rfq.Status != RfqStatus.Published && invitation.Rfq.Status != RfqStatus.QuotesReceived)
        {
            return QuoteSaveResponse.Failure("RFQ is not open for quoting.");
        }

        if (invitation.Rfq.SubmissionDeadline <= DateTime.UtcNow)
        {
            return QuoteSaveResponse.Failure("Submission deadline has passed.");
        }

        if (request.LineItems.Count == 0)
        {
            return QuoteSaveResponse.Failure("Quote requires at least one priced line item.");
        }

        List<LineItem> rfqLineItems = await _context.LineItems
            .Where(li => li.RfqId == request.RfqId)
            .ToListAsync();

        if (rfqLineItems.Count == 0)
        {
            return QuoteSaveResponse.Failure("RFQ has no line items.");
        }

        Dictionary<Guid, LineItem> rfqLineItemMap = rfqLineItems.ToDictionary(li => li.Id);
        bool hasInvalidLineItem = request.LineItems.Any(li => !rfqLineItemMap.ContainsKey(li.LineItemId));
        if (hasInvalidLineItem)
        {
            return QuoteSaveResponse.Failure("One or more quote line items are invalid.");
        }

        bool hasNegativePrice = request.LineItems.Any(li => li.UnitPrice < 0m);
        if (hasNegativePrice)
        {
            return QuoteSaveResponse.Failure("Unit price cannot be negative.");
        }

        Quote? quote = await _context.Quotes
            .Include(q => q.LineItems)
            .FirstOrDefaultAsync(q => q.RfqId == request.RfqId && q.SupplierId == supplier.Id);

        bool isNewQuote = quote == null;
        if (quote == null)
        {
            quote = new Quote
            {
                Id = Guid.NewGuid(),
                RfqId = request.RfqId,
                SupplierId = supplier.Id,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Quotes.AddAsync(quote);
        }

        quote.Status = QuoteStatus.Submitted;
        quote.LeadTimeDays = request.LeadTimeDays;
        quote.ValidUntil = request.ValidUntil.ToUniversalTime();
        quote.PaymentTerms = string.IsNullOrWhiteSpace(request.PaymentTerms) ? null : request.PaymentTerms.Trim();
        quote.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        quote.SubmittedAt = DateTime.UtcNow;

        quote.LineItems.Clear();
        foreach (SaveQuoteLineItemRequest lineItemRequest in request.LineItems)
        {
            LineItem rfqLineItem = rfqLineItemMap[lineItemRequest.LineItemId];
            quote.LineItems.Add(new QuoteLineItem
            {
                Id = Guid.NewGuid(),
                QuoteId = quote.Id,
                LineItemId = rfqLineItem.Id,
                UnitPrice = lineItemRequest.UnitPrice,
                TotalPrice = decimal.Round(lineItemRequest.UnitPrice * rfqLineItem.Quantity, 2),
                Notes = string.IsNullOrWhiteSpace(lineItemRequest.Notes) ? null : lineItemRequest.Notes.Trim(),
                CreatedAt = DateTime.UtcNow
            });
        }

        quote.CalculateTotalAmount();

        invitation.MarkAsQuoted();
        _context.RfqInvitations.Update(invitation);

        if (invitation.Rfq.Status == RfqStatus.Published)
        {
            invitation.Rfq.Status = RfqStatus.QuotesReceived;
        }

        if (isNewQuote)
        {
            await _context.Notifications.AddAsync(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = invitation.Rfq.BuyerId,
                Title = "Quote Submitted",
                Message = $"A supplier submitted a quote for {invitation.Rfq.ReferenceNumber}.",
                Type = "QuoteSubmitted",
                ActionUrl = "/buyer/quotes",
                RelatedEntityId = quote.Id,
                RelatedEntityType = "Quote",
                IsRead = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        await _auditLogService.LogRfqActionAsync(
            request.RfqId,
            supplierUserId,
            isNewQuote ? "QuoteSubmitted" : "QuoteUpdated",
            "Quote",
            quote.Id,
            oldValue: null,
            newValue: $"{{\"totalAmount\":{quote.TotalAmount},\"status\":\"{quote.Status}\"}}",
            details: isNewQuote
                ? $"Supplier submitted quote {quote.Id} for RFQ {invitation.Rfq.ReferenceNumber}."
                : $"Supplier updated quote {quote.Id} for RFQ {invitation.Rfq.ReferenceNumber}.");

        return QuoteSaveResponse.Success(quote.Id);
    }
}
