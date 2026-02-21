using ProQuote.Domain.Enums;

namespace ProQuote.Application.DTOs.Quotes;

/// <summary>
/// Supplier invitation list item.
/// </summary>
public class SupplierInvitationItemDto
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
    /// Gets or sets category name.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets invitation status.
    /// </summary>
    public InvitationStatus Status { get; set; }

    /// <summary>
    /// Gets or sets sent timestamp.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Gets or sets submission deadline.
    /// </summary>
    public DateTime SubmissionDeadline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether quote exists for this RFQ.
    /// </summary>
    public bool HasQuote { get; set; }

    /// <summary>
    /// Gets or sets quote status if quote exists.
    /// </summary>
    public QuoteStatus? QuoteStatus { get; set; }
}

/// <summary>
/// Supplier quote list item.
/// </summary>
public class SupplierQuoteListItemDto
{
    /// <summary>
    /// Gets or sets quote identifier.
    /// </summary>
    public Guid QuoteId { get; set; }

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
    /// Gets or sets quote status.
    /// </summary>
    public QuoteStatus Status { get; set; }

    /// <summary>
    /// Gets or sets total quote amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets submission timestamp.
    /// </summary>
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// Gets or sets quote validity date.
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether quote has been awarded.
    /// </summary>
    public bool IsAwarded { get; set; }
}

/// <summary>
/// Quote editor model.
/// </summary>
public class QuoteEditorDto
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
    public string RfqTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets RFQ currency.
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets RFQ deadline.
    /// </summary>
    public DateTime SubmissionDeadline { get; set; }

    /// <summary>
    /// Gets or sets quote identifier if quote exists.
    /// </summary>
    public Guid? QuoteId { get; set; }

    /// <summary>
    /// Gets or sets lead time in days.
    /// </summary>
    public int LeadTimeDays { get; set; } = 14;

    /// <summary>
    /// Gets or sets quote valid until date.
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Gets or sets payment terms.
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Gets or sets quote notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets quote line items.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list is required for form binding in Blazor.")]
    public List<QuoteEditorLineItemDto> LineItems { get; set; } = [];
}

/// <summary>
/// Quote editor line item.
/// </summary>
public class QuoteEditorLineItemDto
{
    /// <summary>
    /// Gets or sets RFQ line item identifier.
    /// </summary>
    public Guid LineItemId { get; set; }

    /// <summary>
    /// Gets or sets line item name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets line item description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets requested quantity.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets unit of measure.
    /// </summary>
    public string UnitOfMeasure { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets offered unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets line notes.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for quote save.
/// </summary>
public class SaveQuoteRequest
{
    /// <summary>
    /// Gets or sets RFQ identifier.
    /// </summary>
    public Guid RfqId { get; set; }

    /// <summary>
    /// Gets or sets lead time in days.
    /// </summary>
    public int LeadTimeDays { get; set; }

    /// <summary>
    /// Gets or sets quote valid until date (UTC).
    /// </summary>
    public DateTime ValidUntil { get; set; }

    /// <summary>
    /// Gets or sets payment terms.
    /// </summary>
    public string? PaymentTerms { get; set; }

    /// <summary>
    /// Gets or sets notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets line item prices.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list is required for form binding in Blazor.")]
    public List<SaveQuoteLineItemRequest> LineItems { get; set; } = [];
}

/// <summary>
/// Request line item price.
/// </summary>
public class SaveQuoteLineItemRequest
{
    /// <summary>
    /// Gets or sets RFQ line item identifier.
    /// </summary>
    public Guid LineItemId { get; set; }

    /// <summary>
    /// Gets or sets unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets optional notes.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Response model for quote save.
/// </summary>
public class QuoteSaveResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether operation succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets quote identifier.
    /// </summary>
    public Guid? QuoteId { get; set; }

    /// <summary>
    /// Gets or sets error message.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates success response.
    /// </summary>
    /// <param name="quoteId">Quote identifier.</param>
    /// <returns>Response instance.</returns>
    public static QuoteSaveResponse Success(Guid quoteId)
    {
        return new QuoteSaveResponse
        {
            Succeeded = true,
            QuoteId = quoteId
        };
    }

    /// <summary>
    /// Creates failure response.
    /// </summary>
    /// <param name="error">Error message.</param>
    /// <returns>Response instance.</returns>
    public static QuoteSaveResponse Failure(string error)
    {
        return new QuoteSaveResponse
        {
            Succeeded = false,
            ErrorMessage = error
        };
    }
}
