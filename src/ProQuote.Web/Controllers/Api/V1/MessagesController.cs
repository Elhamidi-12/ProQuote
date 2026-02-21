using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProQuote.Application.DTOs.Communication;
using ProQuote.Application.Interfaces;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Messaging endpoints for buyer and supplier users.
/// </summary>
[Route("api/v1/messages")]
[Authorize(Roles = ApplicationRoles.All, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessagesController : ApiControllerBase
{
    private readonly ICommunicationService _communicationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagesController"/> class.
    /// </summary>
    /// <param name="communicationService">Communication service.</param>
    public MessagesController(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    /// <summary>
    /// Gets selectable RFQs for current user message view/composer.
    /// </summary>
    /// <returns>RFQ options for the current role.</returns>
    [HttpGet("rfqs")]
    public async Task<IActionResult> GetRfqOptions()
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        if (User.IsInRole(ApplicationRoles.Buyer))
        {
            IReadOnlyList<MessagingRfqOptionDto> buyerOptions =
                await _communicationService.GetBuyerRfqOptionsAsync(CurrentUserId.Value);
            return Ok(buyerOptions);
        }

        if (User.IsInRole(ApplicationRoles.Supplier))
        {
            IReadOnlyList<MessagingRfqOptionDto> supplierOptions =
                await _communicationService.GetSupplierRfqOptionsAsync(CurrentUserId.Value);
            return Ok(supplierOptions);
        }

        return Forbid();
    }

    /// <summary>
    /// Gets thread messages visible to current user.
    /// </summary>
    /// <param name="rfqId">Optional RFQ filter identifier.</param>
    /// <param name="take">Maximum item count.</param>
    /// <returns>Message thread items.</returns>
    [HttpGet]
    public async Task<IActionResult> GetMessages([FromQuery] Guid? rfqId = null, [FromQuery] int take = 100)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        if (User.IsInRole(ApplicationRoles.Buyer))
        {
            IReadOnlyList<MessageThreadItemDto> buyerMessages =
                await _communicationService.GetBuyerMessagesAsync(CurrentUserId.Value, rfqId, take);
            return Ok(buyerMessages);
        }

        if (User.IsInRole(ApplicationRoles.Supplier))
        {
            IReadOnlyList<MessageThreadItemDto> supplierMessages =
                await _communicationService.GetSupplierMessagesAsync(CurrentUserId.Value, rfqId, take);
            return Ok(supplierMessages);
        }

        return Forbid();
    }

    /// <summary>
    /// Sends a new message as current user.
    /// </summary>
    /// <param name="request">Message payload.</param>
    /// <returns>Operation result.</returns>
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        if (request.RfqId == Guid.Empty)
        {
            return BadRequest(new { succeeded = false, error = "RFQ id is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { succeeded = false, error = "Message is required." });
        }

        bool sent;

        if (User.IsInRole(ApplicationRoles.Buyer))
        {
            sent = await _communicationService.SendBuyerMessageAsync(
                CurrentUserId.Value,
                request.RfqId,
                request.Message,
                request.TargetSupplierId);
            return sent ? Ok(new { succeeded = true }) : BadRequest(new { succeeded = false });
        }

        if (User.IsInRole(ApplicationRoles.Supplier))
        {
            if (request.TargetSupplierId.HasValue)
            {
                return BadRequest(new { succeeded = false, error = "Suppliers cannot set target supplier." });
            }

            sent = await _communicationService.SendSupplierMessageAsync(CurrentUserId.Value, request.RfqId, request.Message);
            return sent ? Ok(new { succeeded = true }) : BadRequest(new { succeeded = false });
        }

        return Forbid();
    }
}
