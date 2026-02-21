using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProQuote.Application.Interfaces;
using ProQuote.Infrastructure.Identity;

namespace ProQuote.Web.Controllers.Api.V1;

/// <summary>
/// Category lookup endpoints for authenticated clients.
/// </summary>
[Route("api/v1/categories")]
[Authorize(Roles = ApplicationRoles.All, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CategoriesController : ApiControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoriesController"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of work.</param>
    public CategoriesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets active categories.
    /// </summary>
    /// <returns>Category list.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        IReadOnlyList<Domain.Entities.Category> categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        return Ok(categories.Select(c => new
        {
            c.Id,
            c.Name,
            c.Description,
            c.ParentCategoryId
        }));
    }
}
