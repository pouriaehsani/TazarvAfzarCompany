using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Application.DTOs.Category;
using FluentValidation;

namespace Company.Application.Validators.Categories
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("نام دسته‌بندی الزامی است.")
                .MaximumLength(100)
                .WithMessage("حداکثر طول نام دسته‌بندی ۱۰۰ کاراکتر است.");
        }
    }
}
