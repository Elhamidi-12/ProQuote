using ProQuote.Application.DTOs.Invitations;

namespace ProQuote.Application.UseCases.BuyerRfqs.Invitations;

/// <summary>
/// Application use-case contract for sending buyer RFQ invitations.
/// </summary>
public interface ISendRfqInvitationsUseCase
{
    /// <summary>
    /// Executes supplier invitation workflow.
    /// </summary>
    /// <param name="command">Invitation command payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Send invitation response.</returns>
    Task<SendRfqInvitationsResponse> ExecuteAsync(
        SendRfqInvitationsCommand command,
        CancellationToken cancellationToken = default);
}
