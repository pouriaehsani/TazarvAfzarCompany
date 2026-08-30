using FluentValidation;
using Company.Application.DTOs.Article;

namespace Company.Application.Validators.Articles
{
    /// <summary>
    /// Validation rules that an article update request must satisfy before the
    /// application layer is allowed to persist the change.
    ///
    /// Unlike create, the image is optional on edit (an existing image is kept
    /// when no new one is uploaded), so only the image's format/size are checked
    /// when a file is actually provided.
    /// </summary>
    public class UpdateArticleValidator : AbstractValidator<UpdateArticleDto>
    {
        public UpdateArticleValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Content)
                .NotEmpty();

            RuleFor(x => x.CategoryId)
                .GreaterThan(0);

            // Image is optional; validate only when a file was provided.
            RuleFor(x => x.Image)
                .Must(file => file == null || file.Length > 0)
                .WithMessage("The image file is empty.")
                .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                .WithMessage("Image size cannot exceed 5 MB.")
                .Must(file =>
                    file == null ||
                    file.ContentType == "image/jpeg" ||
                    file.ContentType == "image/png")
                .WithMessage("Image format must be JPG or PNG.");
        }
    }
}
