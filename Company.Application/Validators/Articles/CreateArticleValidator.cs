using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Application.DTOs.Article;
using FluentValidation;

namespace Company.Application.Validators.Articles
{
    public class CreateArticleValidator : AbstractValidator<CreateArticleDto>
    {
        public CreateArticleValidator()
        {
            RuleFor(x => x.Title)
             .MaximumLength(200);


            RuleFor(x => x.Description)
           .NotEmpty()
           .MaximumLength(500);

            RuleFor(x => x.Content)
                .NotEmpty();

            RuleFor(x => x.CategoryId)
                .GreaterThan(0);

            RuleFor(x => x.TagNames)
                .NotEmpty();

            RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Please select an image.")
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
