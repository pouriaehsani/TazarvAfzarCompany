using Company.Application.DTOs.Article;
using Company.Application.Exceptions;
using Company.Application.Interfaces;
using Company.web.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Company.web.Areas.Admin.Controllers;

[Area("Admin")]
public class ArticleController : Controller
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public IActionResult CreateArticle()
    {
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

            return View(dto);
        }

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
}