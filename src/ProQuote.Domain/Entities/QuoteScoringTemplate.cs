namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents buyer-defined scoring weights used when comparing RFQ quotes.
/// </summary>
public class QuoteScoringTemplate : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the RFQ identifier this template belongs to.
    /// </summary>
    public Guid RfqId { get; set; }

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

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the RFQ that owns this scoring template.
    /// </summary>
    public virtual Rfq Rfq { get; set; } = null!;

    #endregion
}
