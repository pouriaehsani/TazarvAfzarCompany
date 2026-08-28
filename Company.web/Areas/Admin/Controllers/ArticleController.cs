using Company.Application.DTOs.Article;
using Company.Application.Extensions;
using Company.Application.Interfaces;
using Company.Application.Validators.Articles;
using Company.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Company.web.Areas.Admin.Controllers;

[Area("Admin")]
public class ArticleController : Controller
{
    private readonly IArticleService _articleService;
    private readonly IValidator<CreateArticleDto> _validator;

    public ArticleController(IArticleService articleService,IValidator<CreateArticleDto> validator)
    {
        _articleService = articleService;
        _validator = validator;
    }

    [HttpGet]
    public IActionResult CreateArticle()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateArticle(CreateArticleDto dto)
    {
        var result = await _validator.ValidateAsync(dto);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);

            return View(dto);
        }
        await _articleService.CreateAsync(dto);

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