namespace RFQApp.Domain.Entities;

/// <summary>
/// Represents a line item within an RFQ that suppliers will quote on.
/// </summary>
/// <remarks>
/// Each line item represents a specific product or service the buyer needs,
/// with quantity, unit of measure, and optional technical specifications.
/// </remarks>
public class LineItem : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the parent RFQ.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets the name of the item being requested.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the item.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the required quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure (e.g., kg, pcs, m², hours).
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the technical specifications or requirements.
    /// </summary>
    public string? TechnicalSpecs { get; set; }

    /// <summary>
    /// Gets or sets the display order of this line item within the RFQ.
    /// </summary>
    public int DisplayOrder { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the parent RFQ.
    /// </summary>
    public virtual Rfq Rfq { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of quote line items referencing this line item.
    /// </summary>
    public virtual ICollection<QuoteLineItem> QuoteLineItems { get; set; } = new List<QuoteLineItem>();

    #endregion
}
