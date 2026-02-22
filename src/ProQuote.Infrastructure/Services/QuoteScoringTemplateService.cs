using Microsoft.EntityFrameworkCore;

using ProQuote.Application.DTOs.Quotes;
using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Services;

/// <summary>
/// RFQ quote scoring template service backed by the application database.
/// </summary>
public sealed class QuoteScoringTemplateService : IQuoteScoringTemplateService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteScoringTemplateService"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public QuoteScoringTemplateService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<QuoteScoringTemplateDto> GetTemplateAsync(Guid buyerUserId, Guid rfqId)
    {
        await EnsureBuyerOwnsRfqAsync(buyerUserId, rfqId);

        QuoteScoringTemplate? template = await _context.QuoteScoringTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RfqId == rfqId);

        if (template != null)
        {
            return new QuoteScoringTemplateDto
            {
                PriceWeight = template.PriceWeight,
                LeadTimeWeight = template.LeadTimeWeight,
                CoverageWeight = template.CoverageWeight
            };
        }

        return new QuoteScoringTemplateDto();
    }

    /// <inheritdoc />
    public async Task<QuoteScoringTemplateDto> SaveTemplateAsync(Guid buyerUserId, Guid rfqId, QuoteScoringTemplateDto scoringTemplate)
    {
        ArgumentNullException.ThrowIfNull(scoringTemplate);
        await EnsureBuyerOwnsRfqAsync(buyerUserId, rfqId);

        QuoteScoringTemplateDto normalized = Normalize(scoringTemplate);
        QuoteScoringTemplate? existing = await _context.QuoteScoringTemplates
            .FirstOrDefaultAsync(x => x.RfqId == rfqId);

        if (existing == null)
        {
            await _context.QuoteScoringTemplates.AddAsync(new QuoteScoringTemplate
            {
                Id = Guid.NewGuid(),
                RfqId = rfqId,
                PriceWeight = normalized.PriceWeight,
                LeadTimeWeight = normalized.LeadTimeWeight,
                CoverageWeight = normalized.CoverageWeight
            });
        }
        else
        {
            existing.PriceWeight = normalized.PriceWeight;
            existing.LeadTimeWeight = normalized.LeadTimeWeight;
            existing.CoverageWeight = normalized.CoverageWeight;
        }

        await _context.SaveChangesAsync();
        return normalized;
    }

    private async Task EnsureBuyerOwnsRfqAsync(Guid buyerUserId, Guid rfqId)
    {
        bool exists = await _context.Rfqs.AnyAsync(r => r.Id == rfqId && r.BuyerId == buyerUserId);
        if (!exists)
        {
            throw new InvalidOperationException("RFQ not found.");
        }
    }

    private static QuoteScoringTemplateDto Normalize(QuoteScoringTemplateDto template)
    {
        decimal price = Clamp(template.PriceWeight);
        decimal lead = Clamp(template.LeadTimeWeight);
        decimal coverage = Clamp(template.CoverageWeight);

        decimal total = price + lead + coverage;
        if (total <= 0m)
        {
            return new QuoteScoringTemplateDto();
        }

        return new QuoteScoringTemplateDto
        {
            PriceWeight = Math.Round(price * 100m / total, 2),
            LeadTimeWeight = Math.Round(lead * 100m / total, 2),
            CoverageWeight = Math.Round(coverage * 100m / total, 2)
        };
    }

    private static decimal Clamp(decimal value)
    {
        if (value < 0m)
        {
            return 0m;
        }

        if (value > 100m)
        {
            return 100m;
        }

        return value;
    }

}
