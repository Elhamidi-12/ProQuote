using ProQuote.Domain.Enums;

namespace ProQuote.Application.DTOs.Invitations;

/// <summary>
/// Buyer RFQ invitation context payload.
/// </summary>
public sealed class BuyerRfqInvitationContextDto
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
    /// Gets or sets category identifier.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets category display name.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets submission deadline.
    /// </summary>
    public DateTime SubmissionDeadline { get; set; }

    /// <summary>
    /// Gets or sets supplier candidates.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies UI binding and serialization.")]
    public List<BuyerInvitationSupplierCandidateDto> Candidates { get; set; } = [];
}

/// <summary>
/// Supplier candidate for invitation.
/// </summary>
public sealed class BuyerInvitationSupplierCandidateDto
{
    /// <summary>
    /// Gets or sets supplier identifier.
    /// </summary>
    public Guid SupplierId { get; set; }

    /// <summary>
    /// Gets or sets supplier company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets supplier contact name.
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets supplier email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether supplier matches RFQ category.
    /// </summary>
    public bool CategoryMatch { get; set; }

    /// <summary>
    /// Gets or sets whether supplier already has invitation.
    /// </summary>
    public bool AlreadyInvited { get; set; }

    /// <summary>
    /// Gets or sets existing invitation status.
    /// </summary>
    public InvitationStatus? InvitationStatus { get; set; }
}

/// <summary>
/// Request payload to send RFQ invitations.
/// </summary>
public sealed class SendRfqInvitationsRequest
{
    /// <summary>
    /// Gets or sets supplier ids to invite.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies UI/API model binding.")]
    public List<Guid> SupplierIds { get; set; } = [];
}

/// <summary>
/// RFQ invitation send response payload.
/// </summary>
public sealed class SendRfqInvitationsResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether request succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets sent invitations count.
    /// </summary>
    public int SentCount { get; set; }

    /// <summary>
    /// Gets or sets skipped suppliers count.
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// Gets or sets optional message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Creates success response.
    /// </summary>
    public static SendRfqInvitationsResponse Success(int sentCount, int skippedCount, string? message = null)
    {
        return new SendRfqInvitationsResponse
        {
            Succeeded = true,
            SentCount = sentCount,
            SkippedCount = skippedCount,
            Message = message
        };
    }

    /// <summary>
    /// Creates failure response.
    /// </summary>
    public static SendRfqInvitationsResponse Failure(string message)
    {
        return new SendRfqInvitationsResponse
        {
            Succeeded = false,
            Message = message
        };
    }
}
