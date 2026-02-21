using ProQuote.Domain.Enums;

namespace ProQuote.Domain.Entities;

/// <summary>
/// Represents a supplier company that can receive RFQ invitations and submit quotes.
/// </summary>
/// <remarks>
/// Suppliers must be approved by an admin before they can participate in RFQs.
/// Each supplier is linked to a user account for authentication.
/// </remarks>
public class Supplier : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the associated user account.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the primary contact person's name.
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the primary email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the company website URL.
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the company address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the tax identification number.
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Gets or sets the registration and approval status.
    /// </summary>
    public SupplierStatus Status { get; set; } = SupplierStatus.Pending;

    /// <summary>
    /// Gets or sets the average rating from buyers (0-5 scale).
    /// </summary>
    public double AverageRating { get; set; }

    /// <summary>
    /// Gets or sets the total number of ratings received.
    /// </summary>
    public int TotalRatings { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the supplier registered.
    /// </summary>
    public DateTime RegisteredAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the supplier was approved.
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Gets or sets the reason for rejection or suspension.
    /// </summary>
    public string? StatusReason { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Gets or sets the associated user account.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of categories this supplier operates in.
    /// </summary>
    public virtual ICollection<SupplierCategory> Categories { get; set; } = new List<SupplierCategory>();

    /// <summary>
    /// Gets or sets the collection of quotes submitted by this supplier.
    /// </summary>
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    /// <summary>
    /// Gets or sets the collection of RFQ invitations received by this supplier.
    /// </summary>
    public virtual ICollection<RfqInvitation> Invitations { get; set; } = new List<RfqInvitation>();

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether the supplier can participate in RFQs.
    /// </summary>
    /// <returns><c>true</c> if the supplier is approved; otherwise, <c>false</c>.</returns>
    public bool CanParticipateInRfqs()
    {
        return Status == SupplierStatus.Approved;
    }

    /// <summary>
    /// Adds a new rating and recalculates the average.
    /// </summary>
    /// <param name="rating">The rating value (1-5).</param>
    public void AddRating(int rating)
    {
        double totalScore = AverageRating * TotalRatings;
        TotalRatings++;
        AverageRating = (totalScore + rating) / TotalRatings;
    }

    #endregion
}
