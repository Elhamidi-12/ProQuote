using ProQuote.Application.DTOs.Quotes;
using ProQuote.Application.Interfaces;

namespace ProQuote.Application.UseCases.BuyerRfqs.AwardQuote;

/// <summary>
/// Application use-case implementation for awarding buyer RFQ quotes.
/// </summary>
public sealed class AwardBuyerQuoteUseCase : IAwardBuyerQuoteUseCase
{
    private readonly IBuyerQuoteManagementService _buyerQuoteManagementService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AwardBuyerQuoteUseCase"/> class.
    /// </summary>
    /// <param name="buyerQuoteManagementService">Buyer quote workflow service.</param>
    public AwardBuyerQuoteUseCase(IBuyerQuoteManagementService buyerQuoteManagementService)
    {
        _buyerQuoteManagementService = buyerQuoteManagementService;
    }

    /// <inheritdoc />
    public async Task<AwardQuoteResponse> ExecuteAsync(AwardBuyerQuoteCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.BuyerUserId == Guid.Empty || command.RfqId == Guid.Empty || command.QuoteId == Guid.Empty)
        {
            return AwardQuoteResponse.Failure(command.RfqId, "Invalid award request.");
        }

        return await _buyerQuoteManagementService.AwardQuoteAsync(
            command.BuyerUserId,
            command.RfqId,
            command.QuoteId,
            command.BuyerNotes);
    }
}
