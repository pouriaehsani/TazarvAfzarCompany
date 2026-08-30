using Company.Application.DTOs.Category;
using Company.Application.Exceptions;
using Company.Application.Interfaces;
using Company.web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Company.web.Areas.Admin.Controllers
{
    /// <summary>
    /// Handles HTTP requests for base information management (categories and
    /// tags).
    ///
    /// This controller contains no business logic. Every operation is
    /// delegated to <see cref="IBaseInfo"/> and the outcomes are mapped to the
    /// appropriate HTTP responses, preserving Clean Architecture separation.
    /// </summary>
    [Area("Admin")]
    public class BaseInfoController : Controller
    {
        private readonly IBaseInfo _baseInfoService;

        public BaseInfoController(IBaseInfo baseInfoService)
        {
            _baseInfoService = baseInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> BaseInfo()
        {
            var model = new BaseInfoViewModel
            {
                Categories = await _baseInfoService.GetAllAsync(),
                Tags = await _baseInfoService.GetAllTagsAsync()
            };

            return View(model);
        }

        /// <summary>
        /// Stores a new category posted from the BaseInfo screen.
        /// All business rules are enforced inside the application layer.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromForm] CreateCategoryDto dto)
        {
            try
            {
                await _baseInfoService.CreateAsync(dto);
            }
            catch (Exception ex) when (TryTranslate(ex, out var error))
            {
                return error;
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Could not save the category. Please try again."
                });
            }

            return Json(new
            {
                ok = true,
                message = "Category saved.",
                category = new
                {
                    title = (dto.Title ?? string.Empty).Trim()
                }
            });
        }

        /// <summary>
        /// Renames an existing category. The application layer validates the
        /// input, ensures the new title is unique, and updates the entity.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            [FromForm] UpdateCategoryDto dto)
        {
            try
            {
                await _baseInfoService.UpdateAsync(dto);
            }
            catch (Exception ex) when (TryTranslate(ex, out var error))
            {
                return error;
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Could not update the category. Please try again."
                });
            }

            return Json(new
            {
                ok = true,
                message = "Category updated.",
                category = new
                {
                    id = dto.Id,
                    title = (dto.Title ?? string.Empty).Trim()
                }
            });
        }

        /// <summary>
        /// Deletes a category. The application layer refuses to delete a
        /// category that is still referenced by articles.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _baseInfoService.DeleteAsync(id);
            }
            catch (Exception ex) when (TryTranslate(ex, out var error))
            {
                return error;
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Could not delete the category. Please try again."
                });
            }

            return Json(new
            {
                ok = true,
                message = "Category deleted.",
                id
            });
        }

        /// <summary>
        /// Deletes a tag. Deleting a tag only removes its links from articles
        /// (a pure junction table), so no articles are ever affected.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                await _baseInfoService.DeleteTagAsync(id);
            }
            catch (Exception ex) when (TryTranslate(ex, out var error))
            {
                return error;
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Could not delete the tag. Please try again."
                });
            }

            return Json(new
            {
                ok = true,
                message = "Tag deleted.",
                id
            });
        }

        /// <summary>
        /// Maps application-layer exceptions to their canonical HTTP responses.
        /// Returns <c>true</c> when the exception was a known business-rule
        /// failure (so the caller can short-circuit the generic 500 path).
        /// </summary>
        private bool TryTranslate(
            Exception ex,
            out IActionResult error)
        {
            switch (ex)
            {
                case ValidationException e:
                    error = BadRequest(new
                    {
                        message = string.Join(" ", e.Errors.Select(x => x.ErrorMessage))
                    });
                    return true;

                case DuplicateCategoryNameException e:
                    error = Conflict(new { message = e.Message });
                    return true;

                case CategoryNotFoundException e:
                    error = NotFound(new { message = e.Message });
                    return true;

                case CategoryHasArticlesException e:
                    error = Conflict(new { message = e.Message });
                    return true;

                case TagNotFoundException e:
                    error = NotFound(new { message = e.Message });
                    return true;

                default:
                    error = null!;
                    return false;
            }
        }
    }
}
