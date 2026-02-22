namespace ProQuote.Application.DTOs.Quotes;

/// <summary>
/// Weight configuration used for quote scoring.
/// </summary>
public sealed class QuoteScoringTemplateDto
{
    /// <summary>
    /// Gets or sets weight for price score (0-100).
    /// </summary>
    public decimal PriceWeight { get; set; } = 60m;

    /// <summary>
    /// Gets or sets weight for lead-time score (0-100).
    /// </summary>
    public decimal LeadTimeWeight { get; set; } = 25m;

    /// <summary>
    /// Gets or sets weight for line-coverage score (0-100).
    /// </summary>
    public decimal CoverageWeight { get; set; } = 15m;
}

/// <summary>
/// Score breakdown for a quote.
/// </summary>
public sealed class QuoteScoreBreakdownDto
{
    /// <summary>
    /// Gets or sets price sub-score (0-100).
    /// </summary>
    public decimal PriceScore { get; set; }

    /// <summary>
    /// Gets or sets lead-time sub-score (0-100).
    /// </summary>
    public decimal LeadTimeScore { get; set; }

    /// <summary>
    /// Gets or sets line-coverage sub-score (0-100).
    /// </summary>
    public decimal CoverageScore { get; set; }

    /// <summary>
    /// Gets or sets weighted composite score (0-100).
    /// </summary>
    public decimal CompositeScore { get; set; }
}

/// <summary>
/// Canonicalized quote comparison payload.
/// </summary>
public sealed class CanonicalQuoteComparisonDto
{
    /// <summary>
    /// Gets or sets normalized baselines for RFQ line items.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies API and Blazor serialization.")]
    public List<CanonicalLineBaselineDto> LineBaselines { get; set; } = [];

    /// <summary>
    /// Gets or sets normalized quote entries.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies API and Blazor serialization.")]
    public List<CanonicalQuoteEntryDto> Quotes { get; set; } = [];
}

/// <summary>
/// Baseline metrics for one RFQ line item.
/// </summary>
public sealed class CanonicalLineBaselineDto
{
    /// <summary>
    /// Gets or sets line-item identifier.
    /// </summary>
    public Guid LineItemId { get; set; }

    /// <summary>
    /// Gets or sets line-item name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets median unit price.
    /// </summary>
    public decimal? MedianUnitPrice { get; set; }

    /// <summary>
    /// Gets or sets minimum unit price.
    /// </summary>
    public decimal? MinUnitPrice { get; set; }

    /// <summary>
    /// Gets or sets maximum unit price.
    /// </summary>
    public decimal? MaxUnitPrice { get; set; }
}

/// <summary>
/// Canonicalized quote entry.
/// </summary>
public sealed class CanonicalQuoteEntryDto
{
    /// <summary>
    /// Gets or sets quote identifier.
    /// </summary>
    public Guid QuoteId { get; set; }

    /// <summary>
    /// Gets or sets line coverage percentage (0-100).
    /// </summary>
    public decimal CoveragePercent { get; set; }

    /// <summary>
    /// Gets or sets normalized total (sum of available line totals).
    /// </summary>
    public decimal NormalizedTotalAmount { get; set; }

    /// <summary>
    /// Gets or sets normalized line values.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies API and Blazor serialization.")]
    public List<CanonicalQuoteLineValueDto> Lines { get; set; } = [];
}

/// <summary>
/// Canonicalized line values for a quote.
/// </summary>
public sealed class CanonicalQuoteLineValueDto
{
    /// <summary>
    /// Gets or sets line-item identifier.
    /// </summary>
    public Guid LineItemId { get; set; }

    /// <summary>
    /// Gets or sets unit price.
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets line total.
    /// </summary>
    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets variance against median unit price percentage.
    /// </summary>
    public decimal? VarianceFromMedianPercent { get; set; }
}
