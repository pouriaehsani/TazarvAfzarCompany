using Company.Application.DTOs.Article;
using Company.Application.Exceptions;
using Company.Application.Interfaces;
using Company.web.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Company.web.Areas.Admin.Controllers;

[Area("Admin")]
public class ArticleController : Controller
{
    private readonly IArticleService _articleService;
    private readonly IBaseInfo _baseInfoService;

    public ArticleController(
        IArticleService articleService,
        IBaseInfo baseInfoService)
    {
        _articleService = articleService;
        _baseInfoService = baseInfoService;
    }

    [HttpGet]
    public async Task<IActionResult> CreateArticle()
    {
        await LoadCategoriesAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateArticle(CreateArticleDto dto)
    {
        // Validation is enforced inside IArticleService (application layer);
        // this action only surfaces the validation failures to the view.
        try
        {
            await _articleService.CreateAsync(dto);
        }
        catch (ValidationException ex)
        {
            new ValidationResult(ex.Errors).AddToModelState(ModelState);

            await LoadCategoriesAsync();

            return View(dto);
        }

        // The ArticleList page surfaces a success toast via TempData
        // (rendered in the layout and shown by toast.js after the redirect).
        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Article created.";
        TempData["ToastMessage"] = "Your article was saved successfully.";

        return RedirectToAction(nameof(ArticleList));
    }

    /// <summary>
    /// Loads an article and its tags and returns the editor form (the same
    /// layout as the create form, pre-populated with the article's values).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var article = await _articleService.GetByIdWithTagsAsync(id);
        if (article is null)
        {
            return NotFound();
        }

        var dto = new UpdateArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Description = article.Description,
            Content = article.Content,
            CategoryId = article.CategoryId,
            TagNames = article.ArticleTags
                .Where(at => at.Tag != null)
                .Select(at => at.Tag.Title)
                .ToList()
        };

        // Current image path is read-only display data for the view; it is not
        // part of the submitted DTO.
        ViewBag.ImagePath = article.ImagePath;

        await LoadCategoriesAsync();

        return View(dto);
    }

    /// <summary>
    /// Persists the edited article. All business rules (validation, tag
    /// resolution, image handling) live in the application layer.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Edit(UpdateArticleDto dto)
    {
        try
        {
            await _articleService.UpdateAsync(dto);
        }
        catch (ValidationException ex)
        {
            new ValidationResult(ex.Errors).AddToModelState(ModelState);

            await LoadCategoriesAsync();

            return View(dto);
        }
        catch (ArticleNotFoundException)
        {
            return NotFound();
        }

        TempData["ToastType"] = "success";
        TempData["ToastTitle"] = "Article updated.";
        TempData["ToastMessage"] = "Your article was updated successfully.";

        return RedirectToAction(nameof(ArticleList));
    }

    [HttpGet]
    public async Task<IActionResult> ArticleList()
    {
        var articles = await _articleService.GetAllAsync();

        return View(articles);
    }

    [HttpPost]
    public async Task<IActionResult> UploadContentImage(IFormFile file)
    {
        try
        {
            var imagePath =
                await _articleService.UploadContentImageAsync(file);

            return Ok(new
            {
                location = "/" + imagePath.TrimStart('/')
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Loads the category list for the create/edit form's dropdown.
    /// </summary>
    private async Task LoadCategoriesAsync()
    {
        ViewBag.Categories = await _baseInfoService.GetAllAsync();
    }
}
