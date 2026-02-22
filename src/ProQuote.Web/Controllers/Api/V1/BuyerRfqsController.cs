#pragma warning disable CS1591
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProQuote.Application.DTOs.Invitations;
using ProQuote.Application.DTOs.Quotes;
using ProQuote.Application.Interfaces;
using ProQuote.Application.UseCases.BuyerRfqs.AwardQuote;
using ProQuote.Application.UseCases.BuyerRfqs.Invitations;
using ProQuote.Application.UseCases.BuyerRfqs.PublishRfq;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Buyer RFQ API endpoints.
/// </summary>
[Route("api/v1/buyer/rfqs")]
[Authorize(Roles = ApplicationRoles.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "DTOs are controller-scoped request/response contracts.")]
public class BuyerRfqsController : ApiControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBuyerQuoteManagementService _buyerQuoteManagementService;
    private readonly IBuyerRfqInvitationService _buyerRfqInvitationService;
    private readonly IAwardBuyerQuoteUseCase _awardBuyerQuoteUseCase;
    private readonly ISendRfqInvitationsUseCase _sendRfqInvitationsUseCase;
    private readonly IPublishBuyerRfqUseCase _publishBuyerRfqUseCase;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuyerRfqsController"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="buyerQuoteManagementService">Buyer quote management service.</param>
    /// <param name="buyerRfqInvitationService">Buyer RFQ invitation service.</param>
    /// <param name="awardBuyerQuoteUseCase">Award buyer quote use-case.</param>
    /// <param name="sendRfqInvitationsUseCase">Send RFQ invitations use-case.</param>
    /// <param name="publishBuyerRfqUseCase">Publish buyer RFQ use-case.</param>
    /// <param name="auditLogService">Audit log service.</param>
    public BuyerRfqsController(
        IUnitOfWork unitOfWork,
        IBuyerQuoteManagementService buyerQuoteManagementService,
        IBuyerRfqInvitationService buyerRfqInvitationService,
        IAwardBuyerQuoteUseCase awardBuyerQuoteUseCase,
        ISendRfqInvitationsUseCase sendRfqInvitationsUseCase,
        IPublishBuyerRfqUseCase publishBuyerRfqUseCase,
        IAuditLogService auditLogService)
    {
        _unitOfWork = unitOfWork;
        _buyerQuoteManagementService = buyerQuoteManagementService;
        _buyerRfqInvitationService = buyerRfqInvitationService;
        _awardBuyerQuoteUseCase = awardBuyerQuoteUseCase;
        _sendRfqInvitationsUseCase = sendRfqInvitationsUseCase;
        _publishBuyerRfqUseCase = publishBuyerRfqUseCase;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Gets buyer RFQs.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <returns>RFQ list.</returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] RfqStatus? status = null)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        IReadOnlyList<Rfq> rfqs = await _unitOfWork.Rfqs.GetByBuyerIdAsync(CurrentUserId.Value);
        IEnumerable<Rfq> filtered = status.HasValue ? rfqs.Where(r => r.Status == status.Value) : rfqs;

        return Ok(filtered.Select(r => new BuyerRfqListItemDto
        {
            Id = r.Id,
            ReferenceNumber = r.ReferenceNumber,
            Title = r.Title,
            Status = r.Status,
            SubmissionDeadline = r.SubmissionDeadline,
            CategoryId = r.CategoryId,
            Currency = r.Currency
        }));
    }

    /// <summary>
    /// Gets buyer RFQ details.
    /// </summary>
    /// <param name="id">RFQ identifier.</param>
    /// <returns>RFQ details.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        Rfq? rfq = await _unitOfWork.Rfqs.GetWithDetailsAsync(id);
        if (rfq == null || rfq.BuyerId != CurrentUserId.Value)
        {
            return NotFound();
        }

        return Ok(ToDetails(rfq));
    }

    /// <summary>
    /// Creates buyer RFQ.
    /// </summary>
    /// <param name="request">Create request.</param>
    /// <returns>Created RFQ details.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveBuyerRfqRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Request body is required." });
        }

        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        if (!TryValidateRequest(request, out string validationError))
        {
            return BadRequest(new { message = validationError });
        }

        Rfq rfq = new()
        {
            Id = Guid.NewGuid(),
            ReferenceNumber = await _unitOfWork.Rfqs.GenerateReferenceNumberAsync(),
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            CategoryId = request.CategoryId,
            Currency = request.Currency.Trim().ToUpperInvariant(),
            SubmissionDeadline = request.SubmissionDeadline.ToUniversalTime(),
            Status = RfqStatus.Draft,
            BuyerId = CurrentUserId.Value,
            PublishedAt = null,
            CreatedAt = DateTime.UtcNow
        };

        foreach (LineItemUpsertRequest item in request.LineItems.Select((x, i) => x with { DisplayOrder = i + 1 }))
        {
            rfq.LineItems.Add(new LineItem
            {
                Id = Guid.NewGuid(),
                RfqId = rfq.Id,
                Name = item.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.Trim(),
                Quantity = item.Quantity,
                UnitOfMeasure = item.UnitOfMeasure.Trim(),
                TechnicalSpecs = string.IsNullOrWhiteSpace(item.TechnicalSpecs) ? null : item.TechnicalSpecs.Trim(),
                DisplayOrder = item.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.Rfqs.AddAsync(rfq);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogRfqActionAsync(
            rfq.Id,
            CurrentUserId.Value,
            "RfqCreated",
            "Rfq",
            rfq.Id,
            details: "RFQ draft created via API.",
            ipAddress: RemoteIpAddress);

        if (request.Publish)
        {
            PublishBuyerRfqResponse publishResult = await _publishBuyerRfqUseCase.ExecuteAsync(
                new PublishBuyerRfqCommand(CurrentUserId.Value, rfq.Id, RemoteIpAddress));
            if (!publishResult.Succeeded)
            {
                return BadRequest(new { message = publishResult.ErrorMessage ?? "Unable to publish RFQ." });
            }

            rfq = await _unitOfWork.Rfqs.GetWithDetailsAsync(rfq.Id) ?? rfq;
        }

        return CreatedAtAction(nameof(GetById), new { id = rfq.Id }, ToDetails(rfq));
    }

    /// <summary>
    /// Updates buyer RFQ.
    /// </summary>
    /// <param name="id">RFQ identifier.</param>
    /// <param name="request">Update request.</param>
    /// <returns>Updated RFQ details.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SaveBuyerRfqRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Request body is required." });
        }

        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        if (!TryValidateRequest(request, out string validationError))
        {
            return BadRequest(new { message = validationError });
        }

        Rfq? rfq = await _unitOfWork.Rfqs.GetWithDetailsAsync(id);
        if (rfq == null || rfq.BuyerId != CurrentUserId.Value)
        {
            return NotFound();
        }

        if (!rfq.CanBeEdited())
        {
            return BadRequest(new { message = "Only draft RFQs can be edited." });
        }

        rfq.Title = request.Title.Trim();
        rfq.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        rfq.CategoryId = request.CategoryId;
        rfq.Currency = request.Currency.Trim().ToUpperInvariant();
        rfq.SubmissionDeadline = request.SubmissionDeadline.ToUniversalTime();
        rfq.Status = RfqStatus.Draft;
        rfq.PublishedAt = null;

        rfq.LineItems.Clear();
        foreach (LineItemUpsertRequest item in request.LineItems.Select((x, i) => x with { DisplayOrder = i + 1 }))
        {
            rfq.LineItems.Add(new LineItem
            {
                Id = Guid.NewGuid(),
                RfqId = rfq.Id,
                Name = item.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.Trim(),
                Quantity = item.Quantity,
                UnitOfMeasure = item.UnitOfMeasure.Trim(),
                TechnicalSpecs = string.IsNullOrWhiteSpace(item.TechnicalSpecs) ? null : item.TechnicalSpecs.Trim(),
                DisplayOrder = item.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            });
        }

        _unitOfWork.Rfqs.Update(rfq);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogService.LogRfqActionAsync(
            rfq.Id,
            CurrentUserId.Value,
            "RfqUpdated",
            "Rfq",
            rfq.Id,
            details: "RFQ updated via API.",
            ipAddress: RemoteIpAddress);

        if (request.Publish)
        {
            PublishBuyerRfqResponse publishResult = await _publishBuyerRfqUseCase.ExecuteAsync(
                new PublishBuyerRfqCommand(CurrentUserId.Value, rfq.Id, RemoteIpAddress));
            if (!publishResult.Succeeded)
            {
                return BadRequest(new { message = publishResult.ErrorMessage ?? "Unable to publish RFQ." });
            }

            rfq = await _unitOfWork.Rfqs.GetWithDetailsAsync(rfq.Id) ?? rfq;
        }

        return Ok(ToDetails(rfq));
    }

    /// <summary>
    /// Publishes a buyer RFQ draft.
    /// </summary>
    /// <param name="id">RFQ identifier.</param>
    /// <returns>Publish result.</returns>
    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        PublishBuyerRfqResponse result = await _publishBuyerRfqUseCase.ExecuteAsync(
            new PublishBuyerRfqCommand(CurrentUserId.Value, id, RemoteIpAddress));

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Gets quote comparison payload for buyer RFQ.
    /// </summary>
    /// <param name="id">RFQ identifier.</param>
    /// <returns>Comparison payload.</returns>
    [HttpGet("{id:guid}/quotes/compare")]
    public async Task<IActionResult> GetQuoteComparison(Guid id)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        BuyerQuoteComparisonDto? comparison = await _buyerQuoteManagementService.GetComparisonAsync(CurrentUserId.Value, id);
        return comparison == null ? NotFound() : Ok(comparison);
    }

    /// <summary>
    /// Awards selected quote for buyer RFQ.
    /// </summary>
    /// <param name="id">RFQ identifier.</param>
    /// <param name="quoteId">Quote identifier.</param>
    /// <param name="request">Award request.</param>
    /// <returns>Award result.</returns>
    [HttpPost("{id:guid}/quotes/{quoteId:guid}/award")]
    public async Task<IActionResult> AwardQuote(Guid id, Guid quoteId, [FromBody] AwardQuoteRequest? request = null)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        AwardBuyerQuoteCommand command = new(
            CurrentUserId.Value,
            id,
            quoteId,
            request?.BuyerNotes);

        AwardQuoteResponse result = await _awardBuyerQuoteUseCase.ExecuteAsync(command);

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Gets supplier invitation context for buyer RFQ.
    /// </summary>
    /// <param name="id">RFQ identifier.</param>
    /// <returns>Invitation context.</returns>
    [HttpGet("{id:guid}/invitations/context")]
    public async Task<IActionResult> GetInvitationContext(Guid id)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        BuyerRfqInvitationContextDto? context = await _buyerRfqInvitationService.GetInvitationContextAsync(CurrentUserId.Value, id);
        return context == null ? NotFound() : Ok(context);
    }

    /// <summary>
    /// Sends invitations to selected suppliers.
    /// </summary>
    /// <param name="id">RFQ identifier.</param>
    /// <param name="request">Supplier invitation request.</param>
    /// <returns>Send result.</returns>
    [HttpPost("{id:guid}/invitations/send")]
    public async Task<IActionResult> SendInvitations(Guid id, [FromBody] SendRfqInvitationsRequest request)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        SendRfqInvitationsResponse response = await _sendRfqInvitationsUseCase.ExecuteAsync(
            new SendRfqInvitationsCommand(
                CurrentUserId.Value,
                id,
                request?.SupplierIds ?? []));

        return response.Succeeded ? Ok(response) : BadRequest(response);
    }

    private static bool TryValidateRequest(SaveBuyerRfqRequest request, out string error)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            error = "Title is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            error = "Currency is required.";
            return false;
        }

        if (request.SubmissionDeadline <= DateTime.UtcNow)
        {
            error = "Submission deadline must be in the future.";
            return false;
        }

        if (request.Publish && request.LineItems.Count == 0)
        {
            error = "At least one line item is required to publish.";
            return false;
        }

        if (request.LineItems.Any(li => string.IsNullOrWhiteSpace(li.Name) || string.IsNullOrWhiteSpace(li.UnitOfMeasure)))
        {
            error = "Each line item requires name and unit of measure.";
            return false;
        }

        if (request.LineItems.Any(li => li.Quantity <= 0))
        {
            error = "Line item quantity must be greater than zero.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static BuyerRfqDetailsDto ToDetails(Rfq rfq)
    {
        return new BuyerRfqDetailsDto
        {
            Id = rfq.Id,
            ReferenceNumber = rfq.ReferenceNumber,
            Title = rfq.Title,
            Description = rfq.Description,
            CategoryId = rfq.CategoryId,
            Currency = rfq.Currency,
            SubmissionDeadline = rfq.SubmissionDeadline,
            Status = rfq.Status,
            PublishedAt = rfq.PublishedAt,
            Documents = rfq.Documents
                .OrderBy(d => d.DisplayOrder)
                .Select(d => new BuyerRfqDocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    ContentType = d.ContentType,
                    FileSize = d.FileSize,
                    FilePath = d.FilePath,
                    Description = d.Description,
                    UploadedAt = d.UploadedAt,
                    DisplayOrder = d.DisplayOrder
                }).ToList(),
            LineItems = rfq.LineItems.OrderBy(li => li.DisplayOrder).Select(li => new BuyerRfqLineItemDto
            {
                Id = li.Id,
                Name = li.Name,
                Description = li.Description,
                Quantity = li.Quantity,
                UnitOfMeasure = li.UnitOfMeasure,
                TechnicalSpecs = li.TechnicalSpecs,
                DisplayOrder = li.DisplayOrder
            }).ToList()
        };
    }

    /// <summary>
    /// RFQ list item payload.
    /// </summary>
    public sealed class BuyerRfqListItemDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public RfqStatus Status { get; set; }
        public DateTime SubmissionDeadline { get; set; }
        public Guid CategoryId { get; set; }
        public string Currency { get; set; } = "USD";
    }

    /// <summary>
    /// RFQ details payload.
    /// </summary>
    public sealed class BuyerRfqDetailsDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime SubmissionDeadline { get; set; }
        public RfqStatus Status { get; set; }
        public DateTime? PublishedAt { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies JSON payload construction.")]
        public List<BuyerRfqDocumentDto> Documents { get; set; } = [];
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list simplifies JSON payload construction.")]
        public List<BuyerRfqLineItemDto> LineItems { get; set; } = [];
    }

    /// <summary>
    /// RFQ document payload.
    /// </summary>
    public sealed class BuyerRfqDocumentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// RFQ line item payload.
    /// </summary>
    public sealed class BuyerRfqLineItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
        public string? TechnicalSpecs { get; set; }
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// RFQ create/update request.
    /// </summary>
    public sealed class SaveBuyerRfqRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime SubmissionDeadline { get; set; }
        public bool Publish { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Mutable list is required for JSON model binding.")]
        public List<LineItemUpsertRequest> LineItems { get; set; } = [];
    }

    /// <summary>
    /// RFQ line item create/update request.
    /// </summary>
    public sealed record LineItemUpsertRequest
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal Quantity { get; init; }
        public string UnitOfMeasure { get; init; } = string.Empty;
        public string? TechnicalSpecs { get; init; }
        public int DisplayOrder { get; init; }
    }
}
#pragma warning restore CS1591
