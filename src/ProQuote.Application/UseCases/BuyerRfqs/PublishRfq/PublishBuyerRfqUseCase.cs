using ProQuote.Application.Interfaces;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.UseCases.BuyerRfqs.PublishRfq;

/// <summary>
/// Application use-case implementation for publishing buyer RFQs.
/// </summary>
public sealed class PublishBuyerRfqUseCase : IPublishBuyerRfqUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishBuyerRfqUseCase"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="auditLogService">Audit log service.</param>
    public PublishBuyerRfqUseCase(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
    {
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    /// <inheritdoc />
    public async Task<PublishBuyerRfqResponse> ExecuteAsync(PublishBuyerRfqCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.BuyerUserId == Guid.Empty || command.RfqId == Guid.Empty)
        {
            return PublishBuyerRfqResponse.Failure(command.RfqId, "Invalid publish request.");
        }

        Domain.Entities.Rfq? rfq = await _unitOfWork.Rfqs.GetWithDetailsAsync(command.RfqId, cancellationToken);
        if (rfq == null || rfq.BuyerId != command.BuyerUserId)
        {
            return PublishBuyerRfqResponse.Failure(command.RfqId, "RFQ not found.");
        }

        if (!rfq.CanBePublished())
        {
            return PublishBuyerRfqResponse.Failure(command.RfqId, "RFQ cannot be published in its current state.");
        }

        rfq.Status = RfqStatus.Published;
        rfq.PublishedAt = DateTime.UtcNow;

        _unitOfWork.Rfqs.Update(rfq);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogRfqActionAsync(
            rfq.Id,
            command.BuyerUserId,
            "RfqPublished",
            "Rfq",
            rfq.Id,
            newValue: $"{{\"status\":\"{rfq.Status}\",\"publishedAt\":\"{rfq.PublishedAt:O}\"}}",
            details: "RFQ published via application use-case.",
            ipAddress: command.RemoteIpAddress);

        return PublishBuyerRfqResponse.Success(rfq.Id, rfq.PublishedAt);
    }
}
