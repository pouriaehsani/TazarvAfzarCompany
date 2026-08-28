using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Company.Application.DTOs.Article;

namespace Company.Application.Validators.Articles
{
    public class UpdateArticleValidator: AbstractValidator<UpdateArticleDto>
    {
        public UpdateArticleValidator() 
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
            
            RuleFor(x=> x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Content)
                .NotEmpty();

            RuleFor(x => x.CategoryId)
                .GreaterThan(0);

            RuleFor(x => x.TagIds)
                .NotEmpty();

            RuleFor(x => x.Image)
                .NotNull();

        }
    }
}
