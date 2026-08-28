using System;

namespace Company.Application.Exceptions;

/// <summary>
/// Thrown when a category with the same title already exists.
/// </summary>
public class DuplicateCategoryNameException : Exception
{
    public DuplicateCategoryNameException(string title)
        : base($"A category named '{title}' already exists.")
    {
        Title = title;
    }

    public string Title { get; }
}
