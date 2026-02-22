using ProQuote.Application.DTOs.Invitations;
using ProQuote.Application.Interfaces;

namespace ProQuote.Application.UseCases.BuyerRfqs.Invitations;

/// <summary>
/// Application use-case implementation for sending buyer RFQ invitations.
/// </summary>
public sealed class SendRfqInvitationsUseCase : ISendRfqInvitationsUseCase
{
    private readonly IBuyerRfqInvitationService _buyerRfqInvitationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendRfqInvitationsUseCase"/> class.
    /// </summary>
    /// <param name="buyerRfqInvitationService">Buyer RFQ invitation service.</param>
    public SendRfqInvitationsUseCase(IBuyerRfqInvitationService buyerRfqInvitationService)
    {
        _buyerRfqInvitationService = buyerRfqInvitationService;
    }

    /// <inheritdoc />
    public async Task<SendRfqInvitationsResponse> ExecuteAsync(
        SendRfqInvitationsCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        cancellationToken.ThrowIfCancellationRequested();

        if (command.BuyerUserId == Guid.Empty || command.RfqId == Guid.Empty)
        {
            return SendRfqInvitationsResponse.Failure("Invalid invitation request.");
        }

        IReadOnlyCollection<Guid> supplierIds = command.SupplierIds ?? [];

        return await _buyerRfqInvitationService.SendInvitationsAsync(
            command.BuyerUserId,
            command.RfqId,
            supplierIds);
    }
}
