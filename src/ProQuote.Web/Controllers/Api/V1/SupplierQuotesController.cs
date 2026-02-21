using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProQuote.Application.DTOs.Quotes;
using ProQuote.Application.Interfaces;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Supplier invitation and quote submission API endpoints.
/// </summary>
[Route("api/v1/supplier")]
[Authorize(Roles = ApplicationRoles.Supplier, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SupplierQuotesController : ApiControllerBase
{
    private readonly IQuoteSubmissionService _quoteSubmissionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupplierQuotesController"/> class.
    /// </summary>
    /// <param name="quoteSubmissionService">Quote submission service.</param>
    public SupplierQuotesController(IQuoteSubmissionService quoteSubmissionService)
    {
        _quoteSubmissionService = quoteSubmissionService;
    }

    /// <summary>
    /// Gets supplier invitations.
    /// </summary>
    /// <param name="status">Optional invitation status.</param>
    /// <returns>Invitation list.</returns>
    [HttpGet("invitations")]
    public async Task<IActionResult> GetInvitations([FromQuery] InvitationStatus? status = null)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        IReadOnlyList<SupplierInvitationItemDto> invitations =
            await _quoteSubmissionService.GetSupplierInvitationsAsync(CurrentUserId.Value, status);
        return Ok(invitations);
    }

    /// <summary>
    /// Gets supplier quotes.
    /// </summary>
    /// <param name="status">Optional quote status filter.</param>
    /// <returns>Supplier quote list.</returns>
    [HttpGet("quotes")]
    public async Task<IActionResult> GetQuotes([FromQuery] QuoteStatus? status = null)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        IReadOnlyList<SupplierQuoteListItemDto> quotes =
            await _quoteSubmissionService.GetSupplierQuotesAsync(CurrentUserId.Value, status);
        return Ok(quotes);
    }

    /// <summary>
    /// Gets quote editor payload for RFQ.
    /// </summary>
    /// <param name="rfqId">RFQ identifier.</param>
    /// <returns>Quote editor details.</returns>
    [HttpGet("quotes/editor/{rfqId:guid}")]
    public async Task<IActionResult> GetQuoteEditor(Guid rfqId)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        QuoteEditorDto? editor = await _quoteSubmissionService.GetQuoteEditorAsync(CurrentUserId.Value, rfqId);
        return editor == null ? NotFound() : Ok(editor);
    }

    /// <summary>
    /// Saves supplier quote.
    /// </summary>
    /// <param name="request">Quote save request.</param>
    /// <returns>Save result.</returns>
    [HttpPost("quotes/save")]
    public async Task<IActionResult> SaveQuote([FromBody] SaveQuoteRequest request)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        QuoteSaveResponse response = await _quoteSubmissionService.SaveQuoteAsync(CurrentUserId.Value, request);
        return response.Succeeded ? Ok(response) : BadRequest(response);
    }
}
