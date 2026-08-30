using Company.Application.DTOs.Category;
using FluentValidation;

namespace Company.Application.Validators.Categories;

/// <summary>
/// Validation rules that a category update request must satisfy before the
/// application layer is allowed to persist the change.
///
/// Keeping validation in a dedicated validator (rather than inline in the
/// controller) preserves the separation of concerns required by Clean
/// Architecture: the controller only translates the result into a response.
/// </summary>
public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryValidator()
    {
        // A valid update must always target an existing, positively-identified
        // category. This rejects accidental zero/default ids from malformed
        // requests before they reach the data layer.
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category id is invalid.");

        // The same title constraints that apply when creating a category also
        // apply when renaming it, so the stored data stays consistent.
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters.");
    }
}
