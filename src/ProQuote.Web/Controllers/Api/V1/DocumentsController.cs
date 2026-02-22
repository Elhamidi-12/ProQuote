using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

using ProQuote.Application.Interfaces;
using ProQuote.Infrastructure.Identity;
using ProQuote.Web.Files;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Authorized document download endpoints.
/// </summary>
[Route("api/v1/documents")]
[Authorize(Roles = ApplicationRoles.All, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DocumentsController : ApiControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileProvider _contentRootFileProvider;
    private const string PrivateUploadsRelativeRoot = "App_Data/uploads";

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentsController"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="hostEnvironment">Host environment.</param>
    public DocumentsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        _unitOfWork = unitOfWork;
        _contentRootFileProvider = hostEnvironment.ContentRootFileProvider;
    }

    /// <summary>
    /// Downloads an RFQ document when user has access.
    /// </summary>
    /// <param name="documentId">Document identifier.</param>
    /// <returns>File stream response.</returns>
    [HttpGet("rfq/{documentId:guid}")]
    public async Task<IActionResult> DownloadRfqDocument(Guid documentId)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        Domain.Entities.RfqDocument? document = await _unitOfWork.Rfqs.GetDocumentByIdAsync(documentId);
        if (document == null)
        {
            return NotFound();
        }

        Domain.Entities.Rfq? rfq = await _unitOfWork.Rfqs.GetByIdAsync(document.RfqId);
        if (rfq == null)
        {
            return NotFound();
        }

        bool canAccess = false;
        if (User.IsInRole(ApplicationRoles.Admin))
        {
            canAccess = true;
        }
        else if (User.IsInRole(ApplicationRoles.Buyer))
        {
            canAccess = rfq.BuyerId == CurrentUserId.Value;
        }
        else if (User.IsInRole(ApplicationRoles.Supplier))
        {
            Domain.Entities.Supplier? supplier = await _unitOfWork.Suppliers.GetByUserIdAsync(CurrentUserId.Value);
            if (supplier != null)
            {
                canAccess = await _unitOfWork.Rfqs.HasSupplierInvitationAsync(rfq.Id, supplier.Id);
            }
        }

        if (!canAccess)
        {
            return Forbid();
        }

        IFileInfo? fileInfo = ResolveRfqDocumentFile(document);
        if (fileInfo == null || !fileInfo.Exists || fileInfo.IsDirectory)
        {
            return NotFound();
        }

        string contentType = string.IsNullOrWhiteSpace(document.ContentType)
            ? "application/octet-stream"
            : document.ContentType;
        string fileName = DocumentFileValidation.SanitizeFileName(document.FileName);
        return File(fileInfo.CreateReadStream(), contentType, fileName);
    }

    /// <summary>
    /// Downloads a quote document when user has access.
    /// </summary>
    /// <param name="documentId">Document identifier.</param>
    /// <returns>File stream response.</returns>
    [HttpGet("quote/{documentId:guid}")]
    public async Task<IActionResult> DownloadQuoteDocument(Guid documentId)
    {
        if (!CurrentUserId.HasValue)
        {
            return Unauthorized();
        }

        Domain.Entities.QuoteDocument? document = await _unitOfWork.Quotes.GetDocumentByIdAsync(documentId);
        if (document == null)
        {
            return NotFound();
        }

        Domain.Entities.Quote? quote = await _unitOfWork.Quotes.GetWithDetailsAsync(document.QuoteId);
        if (quote == null)
        {
            return NotFound();
        }

        bool canAccess = false;
        if (User.IsInRole(ApplicationRoles.Admin))
        {
            canAccess = true;
        }
        else if (User.IsInRole(ApplicationRoles.Buyer))
        {
            Domain.Entities.Rfq? rfq = await _unitOfWork.Rfqs.GetByIdAsync(quote.RfqId);
            canAccess = rfq != null && rfq.BuyerId == CurrentUserId.Value;
        }
        else if (User.IsInRole(ApplicationRoles.Supplier))
        {
            canAccess = quote.Supplier?.UserId == CurrentUserId.Value;
        }

        if (!canAccess)
        {
            return Forbid();
        }

        IFileInfo? fileInfo = ResolveQuoteDocumentFile(document);
        if (fileInfo == null || !fileInfo.Exists || fileInfo.IsDirectory)
        {
            return NotFound();
        }

        string contentType = string.IsNullOrWhiteSpace(document.ContentType)
            ? "application/octet-stream"
            : document.ContentType;
        string fileName = DocumentFileValidation.SanitizeFileName(document.FileName);
        return File(fileInfo.CreateReadStream(), contentType, fileName);
    }

    private IFileInfo? ResolveRfqDocumentFile(Domain.Entities.RfqDocument document)
    {
        return ResolveSafePrivateFile("rfqs", document.RfqId.ToString("N"), document.StoredFileName);
    }

    private IFileInfo? ResolveQuoteDocumentFile(Domain.Entities.QuoteDocument document)
    {
        return ResolveSafePrivateFile("quotes", document.QuoteId.ToString("N"), document.StoredFileName);
    }

    private IFileInfo? ResolveSafePrivateFile(string scope, string entityKey, string? storedFileName)
    {
        if (string.IsNullOrWhiteSpace(scope) ||
            string.IsNullOrWhiteSpace(entityKey) ||
            string.IsNullOrWhiteSpace(storedFileName))
        {
            return null;
        }

        string safeFileName = Path.GetFileName(storedFileName);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            return null;
        }

        string safeRelativePath = Path.Combine(scope, entityKey, safeFileName)
            .Replace('\\', '/');
        string privateRelativePath = $"{PrivateUploadsRelativeRoot}/{safeRelativePath}";
        IFileInfo fileInfo = _contentRootFileProvider.GetFileInfo(privateRelativePath);
        return fileInfo.Exists && !fileInfo.IsDirectory ? fileInfo : null;
    }
}
