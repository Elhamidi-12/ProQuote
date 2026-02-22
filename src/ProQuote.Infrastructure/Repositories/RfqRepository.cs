using Microsoft.EntityFrameworkCore;

using ProQuote.Application.Interfaces;
using ProQuote.Domain.Entities;
using ProQuote.Domain.Enums;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for RFQ-specific data access operations.
/// </summary>
public class RfqRepository : Repository<Rfq>, IRfqRepository
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="RfqRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RfqRepository(AppDbContext context) : base(context)
    {
    }

    #endregion

    #region Query Methods

    /// <inheritdoc />
    public async Task<Rfq?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(r => r.ReferenceNumber == referenceNumber, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Rfq?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Category)
            .Include(r => r.LineItems.OrderBy(li => li.DisplayOrder))
            .Include(r => r.Invitations)
                .ThenInclude(i => i.Supplier)
            .Include(r => r.Documents.OrderBy(d => d.DisplayOrder))
            .Include(r => r.Quotes)
                .ThenInclude(q => q.Supplier)
            .Include(r => r.Quotes)
                .ThenInclude(q => q.LineItems)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Rfq>> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Category)
            .Where(r => r.BuyerId == buyerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Rfq>> GetByBuyerIdWithDetailsAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.BuyerId == buyerId)
            .Include(r => r.Category)
            .Include(r => r.Quotes)
                .ThenInclude(q => q.Supplier)
            .Include(r => r.Invitations)
            .OrderByDescending(r => r.CreatedAt)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Rfq>> GetByStatusAsync(RfqStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Category)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Rfq>> GetBySupplierInvitationAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Category)
            .Include(r => r.LineItems.OrderBy(li => li.DisplayOrder))
            .Where(r => r.Invitations.Any(i => i.SupplierId == supplierId))
            .OrderByDescending(r => r.SubmissionDeadline)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Rfq>> GetByDeadlinePeriodAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Category)
            .Where(r => r.SubmissionDeadline >= startDate && r.SubmissionDeadline <= endDate)
            .OrderBy(r => r.SubmissionDeadline)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Rfq>> GetExpiredRfqsAsync(CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;

        return await DbSet
            .Where(r => r.Status == RfqStatus.Published &&
                        r.SubmissionDeadline < now &&
                        !r.Quotes.Any())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Rfq>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Category)
            .Include(r => r.Quotes)
                .ThenInclude(q => q.Supplier)
            .Include(r => r.Invitations)
            .OrderByDescending(r => r.CreatedAt)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RfqDocument>> GetDocumentsAsync(Guid rfqId, CancellationToken cancellationToken = default)
    {
        return await Context.RfqDocuments
            .Where(d => d.RfqId == rfqId)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RfqDocument?> GetDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await Context.RfqDocuments.FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasSupplierInvitationAsync(Guid rfqId, Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await Context.RfqInvitations.AnyAsync(
            i => i.RfqId == rfqId && i.SupplierId == supplierId,
            cancellationToken);
    }

    #endregion

    #region Reference Number Generation

    /// <inheritdoc />
    public async Task<string> GenerateReferenceNumberAsync(CancellationToken cancellationToken = default)
    {
        int currentYear = DateTime.UtcNow.Year;
        string yearPrefix = $"RFQ-{currentYear}-";

        string? lastReference = await DbSet
            .Where(r => r.ReferenceNumber.StartsWith(yearPrefix))
            .OrderByDescending(r => r.ReferenceNumber)
            .Select(r => r.ReferenceNumber)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;

        if (!string.IsNullOrEmpty(lastReference))
        {
            string numberPart = lastReference.Replace(yearPrefix, string.Empty);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{yearPrefix}{nextNumber:D5}";
    }

    /// <inheritdoc />
    public async Task AddDocumentsAsync(IEnumerable<RfqDocument> documents, CancellationToken cancellationToken = default)
    {
        await Context.RfqDocuments.AddRangeAsync(documents, cancellationToken);
    }

    /// <inheritdoc />
    public void RemoveDocument(RfqDocument document)
    {
        Context.RfqDocuments.Remove(document);
    }

    #endregion
}
