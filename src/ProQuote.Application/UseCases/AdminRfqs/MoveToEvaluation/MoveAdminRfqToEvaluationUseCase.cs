using ProQuote.Application.Interfaces;
using ProQuote.Domain.Enums;

namespace ProQuote.Application.UseCases.AdminRfqs.MoveToEvaluation;

/// <summary>
/// Application use-case implementation for moving RFQs to under evaluation as admin.
/// </summary>
public sealed class MoveAdminRfqToEvaluationUseCase : IMoveAdminRfqToEvaluationUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveAdminRfqToEvaluationUseCase"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="auditLogService">Audit log service.</param>
    public MoveAdminRfqToEvaluationUseCase(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
    {
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    /// <inheritdoc />
    public async Task<MoveAdminRfqToEvaluationResponse> ExecuteAsync(
        MoveAdminRfqToEvaluationCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.AdminUserId == Guid.Empty || command.RfqId == Guid.Empty)
        {
            return MoveAdminRfqToEvaluationResponse.Failure(command.RfqId, "Invalid update request.");
        }

        Domain.Entities.Rfq? rfq = await _unitOfWork.Rfqs.GetByIdAsync(command.RfqId, cancellationToken);
        if (rfq == null)
        {
            return MoveAdminRfqToEvaluationResponse.Failure(command.RfqId, "RFQ not found.");
        }

        if (rfq.Status != RfqStatus.Published && rfq.Status != RfqStatus.QuotesReceived)
        {
            return MoveAdminRfqToEvaluationResponse.Failure(command.RfqId, "RFQ cannot be moved to under evaluation in its current state.");
        }

        rfq.Status = RfqStatus.UnderEvaluation;

        _unitOfWork.Rfqs.Update(rfq);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogRfqActionAsync(
            rfq.Id,
            command.AdminUserId,
            "AdminMarkedUnderEvaluation",
            "Rfq",
            rfq.Id,
            newValue: $"{{\"status\":\"{rfq.Status}\"}}",
            details: "RFQ moved to under evaluation by admin via application use-case.",
            ipAddress: command.RemoteIpAddress);

        return MoveAdminRfqToEvaluationResponse.Success(rfq.Id);
    }
}
