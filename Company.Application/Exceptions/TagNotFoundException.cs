using System;

namespace Company.Application.Exceptions;

/// <summary>
/// Thrown when a delete operation targets a tag that does not exist in the
/// data store.
///
/// A dedicated exception lets the presentation layer translate the
/// application-layer failure into the correct HTTP status code (404) without
/// the controller knowing any persistence details.
/// </summary>
public class TagNotFoundException : Exception
{
    public TagNotFoundException(int id)
        : base($"A tag with id '{id}' was not found.")
    {
        TagId = id;
    }

    /// <summary>Identifier of the tag that could not be found.</summary>
    public int TagId { get; }
}
