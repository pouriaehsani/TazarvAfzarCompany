using Company.Application.DTOs.Category;
using Company.Application.Exceptions;
using Company.Application.Interfaces;
using Company.Application.Validators.Categories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Company.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseInfoController : Controller
    {
        private readonly IBaseInfo _BaseInfoService;
        private readonly IValidator<CreateCategoryDto> _createCategoryValidator;

        public BaseInfoController(
            IBaseInfo BaseInfoService,
            IValidator<CreateCategoryDto> createCategoryValidator)
        {
            _BaseInfoService = BaseInfoService;
            _createCategoryValidator = createCategoryValidator;
        }

        [HttpGet]
        public async Task<IActionResult> BaseInfo()
        {
            var BaseInfo = await _BaseInfoService.GetAllAsync();
            return View(BaseInfo);
        }


        /// <summary>
        /// Stores a new category posted from the BaseInfo screen.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromForm] CreateCategoryDto dto)
        {
            // dto.Title is non-nullable but can arrive as "" when the field is missing.
            var title = (dto.Title ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(title))
            {
                return BadRequest(new
                {
                    message = "نام دسته‌بندی الزامی است."
                });
            }

            var result = await _createCategoryValidator.ValidateAsync(
                new CreateCategoryDto { Title = title });

            if (!result.IsValid)
            {
                return BadRequest(new
                {
                    message = string.Join(" ", result.Errors.Select(x => x.ErrorMessage))
                });
            }

            try
            {
                await _BaseInfoService.CreateAsync(
                    new CreateCategoryDto { Title = title });
            }
            catch (DuplicateCategoryNameException)
            {
                return Conflict(new
                {
                    message = $"دسته‌بندی «{title}» از قبل وجود دارد."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "ذخیره‌ی دسته‌بندی انجام نشد. لطفاً دوباره تلاش کنید."
                });
            }

            return Json(new
            {
                ok = true,
                message = "دسته‌بندی ذخیره شد.",
                category = new
                {
                    title
                }
            });
        }
    }
}
