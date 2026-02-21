using Microsoft.EntityFrameworkCore.Storage;

using ProQuote.Application.Interfaces;
using ProQuote.Infrastructure.Data;

namespace ProQuote.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for managing database transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    #region Fields

    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRfqRepository? _rfqs;
    private IQuoteRepository? _quotes;
    private ISupplierRepository? _suppliers;
    private ICategoryRepository? _categories;

    private bool _disposed;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    #endregion

    #region Repositories

    /// <inheritdoc />
    public IRfqRepository Rfqs => _rfqs ??= new RfqRepository(_context);

    /// <inheritdoc />
    public IQuoteRepository Quotes => _quotes ??= new QuoteRepository(_context);

    /// <inheritdoc />
    public ISupplierRepository Suppliers => _suppliers ??= new SupplierRepository(_context);

    /// <inheritdoc />
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);

    #endregion

    #region Methods

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="UnitOfWork"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
