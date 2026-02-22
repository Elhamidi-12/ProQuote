using ProQuote.Application.Interfaces;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.UseCases.AdminRfqs.CancelRfq;

/// <summary>
/// Application use-case implementation for cancelling RFQs as admin.
/// </summary>
public sealed class CancelAdminRfqUseCase : ICancelAdminRfqUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelAdminRfqUseCase"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="auditLogService">Audit log service.</param>
    public CancelAdminRfqUseCase(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
    {
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    /// <inheritdoc />
    public async Task<CancelAdminRfqResponse> ExecuteAsync(
        CancelAdminRfqCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.AdminUserId == Guid.Empty || command.RfqId == Guid.Empty)
        {
            return CancelAdminRfqResponse.Failure(command.RfqId, "Invalid cancel request.");
        }

        Domain.Entities.Rfq? rfq = await _unitOfWork.Rfqs.GetByIdAsync(command.RfqId, cancellationToken);
        if (rfq == null)
        {
            return CancelAdminRfqResponse.Failure(command.RfqId, "RFQ not found.");
        }

        if (!rfq.CanBeCancelled())
        {
            return CancelAdminRfqResponse.Failure(command.RfqId, "RFQ cannot be cancelled in its current state.");
        }

        string cancellationReason = string.IsNullOrWhiteSpace(command.Reason)
            ? "Cancelled by admin."
            : command.Reason.Trim();

        rfq.Status = RfqStatus.Cancelled;
        rfq.CancellationReason = cancellationReason;

        _unitOfWork.Rfqs.Update(rfq);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogRfqActionAsync(
            rfq.Id,
            command.AdminUserId,
            "AdminCancelledRfq",
            "Rfq",
            rfq.Id,
            newValue: $"{{\"status\":\"{rfq.Status}\"}}",
            details: "RFQ cancelled by admin via application use-case.",
            ipAddress: command.RemoteIpAddress);

        return CancelAdminRfqResponse.Success(rfq.Id, cancellationReason);
    }
}
