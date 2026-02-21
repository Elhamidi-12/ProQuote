namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents a category for classifying RFQs and suppliers.
/// </summary>
/// <remarks>
/// Categories can be hierarchical with parent-child relationships.
/// Suppliers can be associated with multiple categories.
/// </remarks>
public class Category : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent category for hierarchical structure.
    /// Null for top-level categories.
    /// </summary>
    public Guid? ParentCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the display order for sorting categories.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the category is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the parent category.
    /// </summary>
    public virtual Category? ParentCategory { get; set; }

    /// <summary>
    /// Gets or sets the collection of child categories.
    /// </summary>
    public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();

    /// <summary>
    /// Gets or sets the collection of RFQs in this category.
    /// </summary>
    public virtual ICollection<Rfq> Rfqs { get; set; } = new List<Rfq>();

    /// <summary>
    /// Gets or sets the collection of supplier-category associations.
    /// </summary>
    public virtual ICollection<SupplierCategory> SupplierCategories { get; set; } = new List<SupplierCategory>();

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether this is a top-level category.
    /// </summary>
    /// <returns><c>true</c> if the category has no parent; otherwise, <c>false</c>.</returns>
    public bool IsTopLevel()
    {
        return !ParentCategoryId.HasValue;
    }

    #endregion
}
