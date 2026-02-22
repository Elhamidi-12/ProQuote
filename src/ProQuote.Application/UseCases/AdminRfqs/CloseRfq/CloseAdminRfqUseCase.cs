using ProQuote.Application.Interfaces;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.UseCases.AdminRfqs.CloseRfq;

/// <summary>
/// Application use-case implementation for closing RFQs as admin.
/// </summary>
public sealed class CloseAdminRfqUseCase : ICloseAdminRfqUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloseAdminRfqUseCase"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="auditLogService">Audit log service.</param>
    public CloseAdminRfqUseCase(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
    {
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    /// <inheritdoc />
    public async Task<CloseAdminRfqResponse> ExecuteAsync(
        CloseAdminRfqCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.AdminUserId == Guid.Empty || command.RfqId == Guid.Empty)
        {
            return CloseAdminRfqResponse.Failure(command.RfqId, "Invalid close request.");
        }

        Domain.Entities.Rfq? rfq = await _unitOfWork.Rfqs.GetByIdAsync(command.RfqId, cancellationToken);
        if (rfq == null)
        {
            return CloseAdminRfqResponse.Failure(command.RfqId, "RFQ not found.");
        }

        if (rfq.Status != RfqStatus.UnderEvaluation && rfq.Status != RfqStatus.Awarded)
        {
            return CloseAdminRfqResponse.Failure(command.RfqId, "RFQ cannot be closed in its current state.");
        }

        rfq.Status = RfqStatus.Closed;
        rfq.ClosedAt = DateTime.UtcNow;

        _unitOfWork.Rfqs.Update(rfq);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogRfqActionAsync(
            rfq.Id,
            command.AdminUserId,
            "AdminClosedRfq",
            "Rfq",
            rfq.Id,
            newValue: $"{{\"status\":\"{rfq.Status}\",\"closedAt\":\"{rfq.ClosedAt:O}\"}}",
            details: "RFQ closed by admin via application use-case.",
            ipAddress: command.RemoteIpAddress);

        return CloseAdminRfqResponse.Success(rfq.Id, rfq.ClosedAt);
    }
}
