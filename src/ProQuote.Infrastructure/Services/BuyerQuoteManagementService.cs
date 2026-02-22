using Microsoft.EntityFrameworkCore;

using ProQuote.Application.DTOs.Quotes;
using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Services;

/// <summary>
/// Service implementation for buyer quote comparison and award workflows.
/// </summary>
public class BuyerQuoteManagementService : IBuyerQuoteManagementService
{
    private readonly AppDbContext _context;
    private readonly IAuditLogService _auditLogService;
    private readonly IQuoteComparisonCanonicalizationService _quoteComparisonCanonicalizationService;
    private readonly IQuoteScoringTemplateService _quoteScoringTemplateService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuyerQuoteManagementService"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="auditLogService">Audit log service.</param>
    /// <param name="quoteComparisonCanonicalizationService">Quote canonicalization service.</param>
    /// <param name="quoteScoringTemplateService">Quote scoring template service.</param>
    public BuyerQuoteManagementService(
        AppDbContext context,
        IAuditLogService auditLogService,
        IQuoteComparisonCanonicalizationService quoteComparisonCanonicalizationService,
        IQuoteScoringTemplateService quoteScoringTemplateService)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(auditLogService);
        ArgumentNullException.ThrowIfNull(quoteComparisonCanonicalizationService);
        ArgumentNullException.ThrowIfNull(quoteScoringTemplateService);
        _context = context;
        _auditLogService = auditLogService;
        _quoteComparisonCanonicalizationService = quoteComparisonCanonicalizationService;
        _quoteScoringTemplateService = quoteScoringTemplateService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BuyerQuoteRfqListItemDto>> GetBuyerQuoteRfqsAsync(Guid buyerUserId)
    {
        return await _context.Rfqs
            .Where(r => r.BuyerId == buyerUserId)
            .Select(r => new BuyerQuoteRfqListItemDto
            {
                RfqId = r.Id,
                ReferenceNumber = r.ReferenceNumber,
                Title = r.Title,
                Status = r.Status,
                SubmissionDeadline = r.SubmissionDeadline,
                QuoteCount = r.Quotes.Count(q =>
                    q.Status == QuoteStatus.Submitted ||
                    q.Status == QuoteStatus.UnderReview ||
                    q.Status == QuoteStatus.Awarded),
                AwardedQuoteId = r.Quotes
                    .Where(q => q.IsAwarded || q.Status == QuoteStatus.Awarded)
                    .Select(q => (Guid?)q.Id)
                    .FirstOrDefault()
            })
            .Where(r => r.QuoteCount > 0 || r.AwardedQuoteId.HasValue)
            .OrderByDescending(r => r.SubmissionDeadline)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<BuyerQuoteComparisonDto?> GetComparisonAsync(Guid buyerUserId, Guid rfqId)
    {
        Rfq? rfq = await _context.Rfqs
            .Include(r => r.LineItems.OrderBy(li => li.DisplayOrder))
            .Include(r => r.Quotes)
                .ThenInclude(q => q.Supplier)
            .Include(r => r.Quotes)
                .ThenInclude(q => q.LineItems)
            .Include(r => r.Quotes)
                .ThenInclude(q => q.Documents.OrderBy(d => d.DisplayOrder))
            .Include(r => r.Quotes)
                .ThenInclude(q => q.QualityHistory.OrderByDescending(h => h.ScoredAt))
            .FirstOrDefaultAsync(r => r.Id == rfqId && r.BuyerId == buyerUserId);

        if (rfq == null)
        {
            return null;
        }

        List<Quote> quotes = rfq.Quotes
            .Where(q =>
                q.Status == QuoteStatus.Submitted ||
                q.Status == QuoteStatus.UnderReview ||
                q.Status == QuoteStatus.Awarded ||
                q.Status == QuoteStatus.Rejected)
            .OrderByDescending(q => q.IsAwarded)
            .ThenBy(q => q.TotalAmount)
            .ToList();

        BuyerQuoteComparisonDto dto = new()
        {
            RfqId = rfq.Id,
            ReferenceNumber = rfq.ReferenceNumber,
            Title = rfq.Title,
            Status = rfq.Status,
            Currency = rfq.Currency,
            SubmissionDeadline = rfq.SubmissionDeadline,
            RfqLineItems = rfq.LineItems
                .OrderBy(li => li.DisplayOrder)
                .Select(li => new BuyerQuoteComparisonLineItemDto
                {
                    LineItemId = li.Id,
                    Name = li.Name,
                    Quantity = li.Quantity,
                    UnitOfMeasure = li.UnitOfMeasure
                })
                .ToList(),
            Quotes = quotes.Select(q => new BuyerQuoteComparisonItemDto
            {
                QuoteId = q.Id,
                SupplierId = q.SupplierId,
                SupplierName = q.Supplier.CompanyName,
                Status = q.Status,
                IsAwarded = q.IsAwarded || q.Status == QuoteStatus.Awarded,
                TotalAmount = q.TotalAmount,
                LeadTimeDays = q.LeadTimeDays,
                ValidUntil = q.ValidUntil,
                SubmittedAt = q.SubmittedAt,
                PaymentTerms = q.PaymentTerms,
                Notes = q.Notes,
                BuyerNotes = q.BuyerNotes,
                SubmissionQualityScore = q.SubmissionQualityScore,
                SubmissionCompletenessScore = q.SubmissionCompletenessScore,
                SubmissionLeadTimeScore = q.SubmissionLeadTimeScore,
                SubmissionCommercialScore = q.SubmissionCommercialScore,
                SubmissionQualityScoredAt = q.SubmissionQualityScoredAt,
                QualityTimeline = q.QualityHistory
                    .OrderByDescending(h => h.ScoredAt)
                    .Select(h => new QuoteQualityTimelineEntryDto
                    {
                        EventType = h.EventType,
                        OverallScore = h.OverallScore,
                        CompletenessScore = h.CompletenessScore,
                        LeadTimeScore = h.LeadTimeScore,
                        CommercialScore = h.CommercialScore,
                        ScoredAt = h.ScoredAt
                    })
                    .ToList(),
                Prices = q.LineItems
                    .Select(li => new BuyerQuoteComparisonPriceDto
                    {
                        LineItemId = li.LineItemId,
                        UnitPrice = li.UnitPrice,
                        TotalPrice = li.TotalPrice
                    })
                    .ToList(),
                Documents = q.Documents
                    .OrderBy(d => d.DisplayOrder)
                    .Select(d => new BuyerQuoteComparisonDocumentDto
                    {
                        DocumentId = d.Id,
                        FileName = d.FileName,
                        ContentType = d.ContentType,
                        FileSize = d.FileSize,
                        FilePath = d.FilePath
                    })
                    .ToList()
            }).ToList()
        };

        dto.CanonicalComparison = _quoteComparisonCanonicalizationService.Build(dto);
        dto.ScoringTemplate = await _quoteScoringTemplateService.GetTemplateAsync(buyerUserId, rfqId);
        ApplyQuoteScores(dto);
        dto.Quotes = dto.Quotes
            .OrderByDescending(q => q.IsAwarded || q.Status == QuoteStatus.Awarded)
            .ThenByDescending(q => q.Score.CompositeScore)
            .ThenBy(q => q.TotalAmount)
            .ToList();

        return dto;
    }

    /// <inheritdoc />
    public async Task<AwardQuoteResponse> AwardQuoteAsync(Guid buyerUserId, Guid rfqId, Guid quoteId, string? buyerNotes = null)
    {
        Rfq? rfq = await _context.Rfqs
            .Include(r => r.Quotes)
                .ThenInclude(q => q.Supplier)
            .FirstOrDefaultAsync(r => r.Id == rfqId && r.BuyerId == buyerUserId);

        if (rfq == null)
        {
            return AwardQuoteResponse.Failure(rfqId, "RFQ not found.");
        }

        if (rfq.Status == RfqStatus.Cancelled || rfq.Status == RfqStatus.Closed)
        {
            return AwardQuoteResponse.Failure(rfqId, "RFQ is closed for award.");
        }

        if (rfq.Status == RfqStatus.Draft || rfq.Status == RfqStatus.Published)
        {
            return AwardQuoteResponse.Failure(rfqId, "RFQ has not received comparable quotes yet.");
        }

        Quote? selectedQuote = rfq.Quotes.FirstOrDefault(q => q.Id == quoteId);
        if (selectedQuote == null)
        {
            return AwardQuoteResponse.Failure(rfqId, "Selected quote was not found for this RFQ.");
        }

        if (selectedQuote.Status == QuoteStatus.Withdrawn)
        {
            return AwardQuoteResponse.Failure(rfqId, "Withdrawn quotes cannot be awarded.");
        }

        if (selectedQuote.IsAwarded || selectedQuote.Status == QuoteStatus.Awarded)
        {
            return AwardQuoteResponse.Success(rfqId, selectedQuote.Id);
        }

        List<Quote> comparableQuotes = rfq.Quotes
            .Where(q => q.Status == QuoteStatus.Submitted || q.Status == QuoteStatus.UnderReview || q.Status == QuoteStatus.Awarded)
            .ToList();

        if (comparableQuotes.Count == 0)
        {
            return AwardQuoteResponse.Failure(rfqId, "No submitted quotes available for award.");
        }

        DateTime now = DateTime.UtcNow;

        foreach (Quote quote in comparableQuotes)
        {
            bool isWinner = quote.Id == selectedQuote.Id;
            quote.IsAwarded = isWinner;
            quote.Status = isWinner ? QuoteStatus.Awarded : QuoteStatus.Rejected;
            if (isWinner)
            {
                quote.BuyerNotes = string.IsNullOrWhiteSpace(buyerNotes) ? null : buyerNotes.Trim();
            }
        }

        rfq.Status = RfqStatus.Awarded;
        rfq.AwardedAt = now;

        List<Notification> notifications = [];
        foreach (Quote quote in comparableQuotes.Where(q => q.Supplier?.UserId != Guid.Empty))
        {
            bool isWinner = quote.Id == selectedQuote.Id;
            notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = quote.Supplier.UserId,
                Title = isWinner ? "Quote Awarded" : "Quote Decision",
                Message = isWinner
                    ? $"Your quote was awarded for {rfq.ReferenceNumber}."
                    : $"Your quote was not selected for {rfq.ReferenceNumber}.",
                Type = isWinner ? "QuoteAwarded" : "QuoteRejected",
                ActionUrl = "/supplier/quotes",
                RelatedEntityId = quote.Id,
                RelatedEntityType = "Quote",
                IsRead = false,
                SentAt = now,
                CreatedAt = now
            });
        }

        if (notifications.Count > 0)
        {
            await _context.Notifications.AddRangeAsync(notifications);
        }

        await _context.SaveChangesAsync();

        await _auditLogService.LogRfqActionAsync(
            rfq.Id,
            buyerUserId,
            "QuoteAwarded",
            "Quote",
            selectedQuote.Id,
            oldValue: null,
            newValue: $"{{\"awardedQuoteId\":\"{selectedQuote.Id}\"}}",
            details: BuildAwardAuditDetails(rfq.ReferenceNumber, selectedQuote.Id, comparableQuotes.Count - 1, selectedQuote.BuyerNotes));

        return AwardQuoteResponse.Success(rfqId, selectedQuote.Id);
    }

    private static string BuildAwardAuditDetails(string rfqReferenceNumber, Guid awardedQuoteId, int rejectedCount, string? decisionSummary)
    {
        string baseDetails = $"Awarded quote {awardedQuoteId} for RFQ {rfqReferenceNumber}; rejected {rejectedCount} other quote(s).";
        if (string.IsNullOrWhiteSpace(decisionSummary))
        {
            return baseDetails;
        }

        string compactSummary = decisionSummary.Trim();
        if (compactSummary.Length > 1400)
        {
            compactSummary = compactSummary[..1400];
        }

        return $"{baseDetails} Decision summary: {compactSummary}";
    }

    private static void ApplyQuoteScores(BuyerQuoteComparisonDto comparison)
    {
        Dictionary<Guid, CanonicalQuoteEntryDto> canonicalByQuoteId = comparison.CanonicalComparison.Quotes
            .ToDictionary(x => x.QuoteId);

        List<(Guid QuoteId, decimal Value)> totals = canonicalByQuoteId.Values
            .Select(x => (x.QuoteId, x.NormalizedTotalAmount))
            .ToList();

        List<(Guid QuoteId, decimal Value)> leadTimes = comparison.Quotes
            .Select(x => (x.QuoteId, (decimal)x.LeadTimeDays))
            .ToList();

        decimal minTotal = totals.Count == 0 ? 0m : totals.Min(x => x.Value);
        decimal maxTotal = totals.Count == 0 ? 0m : totals.Max(x => x.Value);
        decimal minLead = leadTimes.Count == 0 ? 0m : leadTimes.Min(x => x.Value);
        decimal maxLead = leadTimes.Count == 0 ? 0m : leadTimes.Max(x => x.Value);

        foreach (BuyerQuoteComparisonItemDto quote in comparison.Quotes)
        {
            canonicalByQuoteId.TryGetValue(quote.QuoteId, out CanonicalQuoteEntryDto? canonical);

            decimal coverageScore = canonical?.CoveragePercent ?? 0m;
            decimal priceScore = ScoreLowerIsBetter(canonical?.NormalizedTotalAmount ?? quote.TotalAmount, minTotal, maxTotal);
            decimal leadTimeScore = ScoreLowerIsBetter(quote.LeadTimeDays, minLead, maxLead);

            decimal composite =
                (priceScore * comparison.ScoringTemplate.PriceWeight +
                 leadTimeScore * comparison.ScoringTemplate.LeadTimeWeight +
                 coverageScore * comparison.ScoringTemplate.CoverageWeight) / 100m;

            quote.Score = new QuoteScoreBreakdownDto
            {
                PriceScore = Math.Round(priceScore, 2),
                LeadTimeScore = Math.Round(leadTimeScore, 2),
                CoverageScore = Math.Round(coverageScore, 2),
                CompositeScore = Math.Round(composite, 2)
            };
        }
    }

    private static decimal ScoreLowerIsBetter(decimal value, decimal min, decimal max)
    {
        if (max <= min)
        {
            return 100m;
        }

        decimal score = (max - value) * 100m / (max - min);
        if (score < 0m)
        {
            return 0m;
        }

        if (score > 100m)
        {
            return 100m;
        }

        return score;
    }
}
