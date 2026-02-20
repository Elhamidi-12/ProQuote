using RFQApp.Domain.Entities;

namespace RFQApp.Application.Interfaces;

/// <summary>
/// Repository interface for Category-specific data access operations.
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    #region Query Methods

    /// <summary>
    /// Gets all top-level categories (categories without a parent).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of top-level categories.</returns>
    Task<IReadOnlyList<Category>> GetTopLevelCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all child categories of a parent category.
    /// </summary>
    /// <param name="parentId">The parent category identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of child categories.</returns>
    Task<IReadOnlyList<Category>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a category with all its child categories.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The category with children if found; otherwise, null.</returns>
    Task<Category?> GetWithChildrenAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active categories.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of active categories.</returns>
    Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the full category hierarchy as a flat list with depth information.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of all categories with hierarchy information.</returns>
    Task<IReadOnlyList<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category name already exists at the same level.
    /// </summary>
    /// <param name="name">The category name.</param>
    /// <param name="parentId">The parent category identifier (null for top-level).</param>
    /// <param name="excludeId">The category identifier to exclude from the check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the name exists; otherwise, false.</returns>
    Task<bool> NameExistsAsync(string name, Guid? parentId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    #endregion
}
