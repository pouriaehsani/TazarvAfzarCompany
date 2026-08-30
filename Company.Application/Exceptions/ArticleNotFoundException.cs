using System;

namespace Company.Application.Exceptions;

/// <summary>
/// Thrown when an update (or other operation) targets an article that does not
/// exist in the data store.
///
/// A dedicated exception lets the presentation layer translate the
/// application-layer failure into the correct HTTP status code (404) without
/// the controller knowing any persistence details.
/// </summary>
public class ArticleNotFoundException : Exception
{
    public ArticleNotFoundException(int id)
        : base($"An article with id '{id}' was not found.")
    {
        ArticleId = id;
    }

    /// <summary>Identifier of the article that could not be found.</summary>
    public int ArticleId { get; }
}
