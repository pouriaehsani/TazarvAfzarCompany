using System;

namespace Company.Application.Exceptions;

/// <summary>
/// Thrown when a category that is still referenced by one or more articles
/// is being deleted.
///
/// The Category -> Article relationship is configured with cascade delete in
/// the data store. Without this guard, deleting an in-use category would
/// silently remove every article that belongs to it. This exception turns
/// that accidental data loss into an explicit, user-facing business rule.
/// </summary>
public class CategoryHasArticlesException : Exception
{
    public CategoryHasArticlesException(int categoryId, string title)
        : base($"The category '{title}' is in use and cannot be deleted.")
    {
        CategoryId = categoryId;
        Title = title;
    }

    /// <summary>Identifier of the category that is still in use.</summary>
    public int CategoryId { get; }

    /// <summary>Title of the category that is still in use.</summary>
    public string Title { get; }
}
