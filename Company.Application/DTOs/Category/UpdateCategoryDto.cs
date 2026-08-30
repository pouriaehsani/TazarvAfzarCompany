namespace Company.Application.DTOs.Category;

/// <summary>
/// Data transfer object used to update an existing category.
///
/// It carries the identifier of the category to change together with the
/// user-editable title coming from the presentation layer. The DTO is the
/// only category data that the web layer is allowed to pass into the
/// application layer — domain entities never cross this boundary.
/// </summary>
public class UpdateCategoryDto
{
    /// <summary>Identifier of the category that must be updated.</summary>
    public int Id { get; set; }

    /// <summary>New display name for the category.</summary>
    public string Title { get; set; } = string.Empty;
}
