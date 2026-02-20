namespace RFQApp.Domain.Entities;

/// <summary>
/// Represents a supplier's price for a specific line item in their quote.
/// </summary>
/// <remarks>
/// Each QuoteLineItem corresponds to one LineItem in the RFQ and contains
/// the supplier's proposed unit price and calculated total price.
/// </remarks>
public class QuoteLineItem : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the parent quote.
    /// </summary>
    public Guid QuoteId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the RFQ line item being quoted.
    /// </summary>
    public Guid LineItemId { get; set; }

    /// <summary>
    /// Gets or sets the price per unit offered by the supplier.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the total price (UnitPrice × Quantity from LineItem).
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets additional notes specific to this line item.
    /// </summary>
    public string? Notes { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the parent quote.
    /// </summary>
    public virtual Quote Quote { get; set; } = null!;

    /// <summary>
    /// Gets or sets the RFQ line item being quoted.
    /// </summary>
    public virtual LineItem LineItem { get; set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Calculates the total price based on unit price and line item quantity.
    /// </summary>
    public void CalculateTotalPrice()
    {
        if (LineItem != null)
        {
            TotalPrice = UnitPrice * LineItem.Quantity;
        }
    }

    #endregion
}
