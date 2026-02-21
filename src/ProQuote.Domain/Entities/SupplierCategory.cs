namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between suppliers and categories.
/// </summary>
/// <remarks>
/// This join entity allows suppliers to be associated with multiple categories
/// they can provide products or services in.
/// </remarks>
public class SupplierCategory
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the supplier.
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the category.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a primary category for the supplier.
    /// </summary>
    public bool IsPrimary { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the supplier.
    /// </summary>
    public virtual Supplier Supplier { get; set; } = null!;

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    public virtual Category Category { get; set; } = null!;

    #endregion
}
