using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Company.Application.DTOs.Article;

/// <summary>
/// Data transfer object used to update an existing article.
///
/// It mirrors the fields of the create form so the same editor UI can be used
/// for both operations. The image is optional: when a new file is supplied it
/// replaces the stored one; when it is null the existing image is kept.
///
/// Tags travel as names (like on create) rather than ids, because the editor
/// works with tag names via Tagify; the application layer resolves and
/// persists them.
/// </summary>
public class UpdateArticleDto
{
    /// <summary>Identifier of the article that must be updated.</summary>
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    /// <summary>New image file; leave null to keep the current one.</summary>
    public IFormFile? Image { get; set; }

    public List<string> TagNames { get; set; } = new();
}
