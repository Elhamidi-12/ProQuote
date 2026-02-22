namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents a historical snapshot of quote quality scores.
/// </summary>
public class QuoteQualityHistory : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the related quote identifier.
    /// </summary>
    public Guid QuoteId { get; set; }

    /// <summary>
    /// Gets or sets the overall quality score.
    /// </summary>
    public int OverallScore { get; set; }

    /// <summary>
    /// Gets or sets the completeness quality score.
    /// </summary>
    public int CompletenessScore { get; set; }

    /// <summary>
    /// Gets or sets the lead-time quality score.
    /// </summary>
    public int LeadTimeScore { get; set; }

    /// <summary>
    /// Gets or sets the commercial quality score.
    /// </summary>
    public int CommercialScore { get; set; }

    /// <summary>
    /// Gets or sets the trigger event type (Submitted, Updated, DocumentsUpdated, etc.).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets UTC timestamp when this snapshot was calculated.
    /// </summary>
    public DateTime ScoredAt { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the related quote.
    /// </summary>
    public virtual Quote Quote { get; set; } = null!;

    #endregion
}
