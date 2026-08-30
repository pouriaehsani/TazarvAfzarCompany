using System;

namespace Company.Application.Exceptions;

/// <summary>
/// Thrown when an operation (update or delete) targets a category that does
/// not exist in the data store.
///
/// Using a dedicated exception lets the presentation layer translate the
/// application-layer failure into the correct HTTP status code (404)
/// without the controller knowing any persistence details.
/// </summary>
public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(int id)
        : base($"A category with id '{id}' was not found.")
    {
        CategoryId = id;
    }

    /// <summary>Identifier of the category that could not be found.</summary>
    public int CategoryId { get; }
}
