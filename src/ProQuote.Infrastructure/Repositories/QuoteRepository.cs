using Microsoft.EntityFrameworkCore;

using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Quote-specific data access operations.
/// </summary>
public class QuoteRepository : Repository<Quote>, IQuoteRepository
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="QuoteRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public QuoteRepository(AppDbContext context) : base(context)
    {
    }

    #endregion

    #region Query Methods

    /// <inheritdoc />
    public async Task<Quote?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Supplier)
            .Include(q => q.Rfq)
                .ThenInclude(r => r.LineItems.OrderBy(li => li.DisplayOrder))
            .Include(q => q.LineItems)
                .ThenInclude(qli => qli.LineItem)
            .Include(q => q.Documents.OrderBy(d => d.DisplayOrder))
            .AsSplitQuery()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Quote>> GetByRfqIdAsync(Guid rfqId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Supplier)
            .Include(q => q.LineItems)
            .Where(q => q.RfqId == rfqId)
            .OrderByDescending(q => q.SubmittedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Quote>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Rfq)
                .ThenInclude(r => r.Category)
            .Where(q => q.SupplierId == supplierId)
            .OrderByDescending(q => q.SubmittedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Quote?> GetByRfqAndSupplierAsync(
        Guid rfqId,
        Guid supplierId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.LineItems)
            .Include(q => q.Documents)
            .FirstOrDefaultAsync(q => q.RfqId == rfqId && q.SupplierId == supplierId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Quote>> GetByStatusAsync(QuoteStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Supplier)
            .Include(q => q.Rfq)
            .Where(q => q.Status == status)
            .OrderByDescending(q => q.SubmittedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Quote?> GetAwardedQuoteForRfqAsync(Guid rfqId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Supplier)
            .Include(q => q.LineItems)
            .FirstOrDefaultAsync(q => q.RfqId == rfqId && q.IsAwarded, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Quote>> GetQuotesForComparisonAsync(Guid rfqId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(q => q.Supplier)
            .Include(q => q.LineItems)
                .ThenInclude(qli => qli.LineItem)
            .Where(q => q.RfqId == rfqId && q.Status == QuoteStatus.Submitted)
            .OrderBy(q => q.TotalAmount)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<QuoteDocument>> GetDocumentsAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        return await Context.QuoteDocuments
            .Where(d => d.QuoteId == quoteId)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<QuoteDocument?> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await Context.QuoteDocuments.FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddDocumentsAsync(IEnumerable<QuoteDocument> documents, CancellationToken cancellationToken = default)
    {
        await Context.QuoteDocuments.AddRangeAsync(documents, cancellationToken);
    }

    /// <inheritdoc />
    public void RemoveDocument(QuoteDocument document)
    {
        Context.QuoteDocuments.Remove(document);
    }

    #endregion
}
