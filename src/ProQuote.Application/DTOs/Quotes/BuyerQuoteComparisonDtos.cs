using ProQuote.Domain.Enums;

namespace ProQuote.Application.DTOs.Quotes;

/// <summary>
/// Buyer quotes list item grouped by RFQ.
/// </summary>
public sealed class BuyerQuoteRfqListItemDto
{
    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets RFQ reference number.
    /// </summary>
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets RFQ title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets RFQ status.
    /// </summary>
    public RfqStatus Status { get; set; }

    /// <summary>
    /// Gets or sets RFQ submission deadline.
    /// </summary>
    public DateTime SubmissionDeadline { get; set; }

    /// <summary>
    /// Gets or sets total submitted quotes count.
    /// </summary>
    public int QuoteCount { get; set; }

    /// <summary>
    /// Gets or sets awarded quote identifier when available.
    /// </summary>
    public Guid? AwardedQuoteId { get; set; }
}

/// <summary>
/// Buyer comparison payload for RFQ.
/// </summary>
public sealed class BuyerQuoteComparisonDto
{
    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets RFQ reference number.
    /// </summary>
    public string ReferenceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets RFQ title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets RFQ status.
    /// </summary>
    public RfqStatus Status { get; set; }

    /// <summary>
    /// Gets or sets RFQ currency.
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets submission deadline.
    /// </summary>
    public DateTime SubmissionDeadline { get; set; }

    /// <summary>
    /// Gets or sets line items for shared comparison reference.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies Blazor/API serialization.")]
    public List<BuyerQuoteComparisonLineItemDto> RfqLineItems { get; set; } = [];

    /// <summary>
    /// Gets or sets submitted quotes for comparison.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies Blazor/API serialization.")]
    public List<BuyerQuoteComparisonItemDto> Quotes { get; set; } = [];
}

/// <summary>
/// RFQ line item comparison metadata.
/// </summary>
public sealed class BuyerQuoteComparisonLineItemDto
{
    /// <summary>
    /// Gets or sets line item identifier.
    /// </summary>
    public Guid LineItemId { get; set; }

    /// <summary>
    /// Gets or sets line item name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets requested quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets unit of measure.
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;
}

/// <summary>
/// Quote comparison item.
/// </summary>
public sealed class BuyerQuoteComparisonItemDto
{
    /// <summary>
    /// Gets or sets quote identifier.
    /// </summary>
    public Guid QuoteId { get; set; }

    /// <summary>
    /// Gets or sets supplier identifier.
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Gets or sets supplier company name.
    /// </summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets quote status.
    /// </summary>
    public QuoteStatus Status { get; set; }

    /// <summary>
    /// Gets or sets whether quote is awarded.
    /// </summary>
    public bool IsAwarded { get; set; }

    /// <summary>
    /// Gets or sets total amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets lead time in days.
    /// </summary>
    public int LeadTimeDays { get; set; }

    /// <summary>
    /// Gets or sets valid-until timestamp.
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Gets or sets submission timestamp.
    /// </summary>
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// Gets or sets payment terms.
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Gets or sets supplier notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets buyer notes.
    /// </summary>
    public string? BuyerNotes { get; set; }

    /// <summary>
    /// Gets or sets line level prices.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies Blazor/API serialization.")]
    public List<BuyerQuoteComparisonPriceDto> Prices { get; set; } = [];
}

/// <summary>
/// Line item price inside a quote comparison payload.
/// </summary>
public sealed class BuyerQuoteComparisonPriceDto
{
    /// <summary>
    /// Gets or sets line item identifier.
    /// </summary>
    public Guid LineItemId { get; set; }

    /// <summary>
    /// Gets or sets unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets line total price.
    /// </summary>
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// Award quote request payload.
/// </summary>
public sealed class AwardQuoteRequest
{
    /// <summary>
    /// Gets or sets buyer private notes on the awarded quote.
    /// </summary>
    public string? BuyerNotes { get; set; }
}

/// <summary>
/// Result payload for award action.
/// </summary>
public sealed class AwardQuoteResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether award succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets awarded quote identifier.
    /// </summary>
    public Guid? QuoteId { get; set; }

    /// <summary>
    /// Gets or sets error message for failures.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates success response.
    /// </summary>
    public static AwardQuoteResponse Success(Guid rfqId, Guid quoteId)
    {
        return new AwardQuoteResponse
        {
            Succeeded = true,
            RfqId = rfqId,
            QuoteId = quoteId
        };
    }

    /// <summary>
    /// Creates failure response.
    /// </summary>
    public static AwardQuoteResponse Failure(Guid rfqId, string errorMessage)
    {
        return new AwardQuoteResponse
        {
            Succeeded = false,
            RfqId = rfqId,
            ErrorMessage = errorMessage
        };
    }
}
