using Company.Application.DTOs.Category;
using Company.Application.Exceptions;
using Company.Application.Interfaces;
using Company.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Both FluentValidation and this project define a type named ValidationException,
// so a bare reference would be ambiguous and fail to compile. The alias removes
// the ambiguity for good and keeps the app's exception flowing to the
// controller's TryTranslate handler (which only understands the app type).
using AppValidationException = Company.Application.Exceptions.ValidationException;

namespace Company.Application.Services
{
    /// <summary>
    /// Application-layer service that owns all business rules for managing
    /// categories.
    ///
    /// The controller delegates every operation here, so it holds zero
    /// business logic — this service is the single place where validation,
    /// uniqueness, and referential-integrity rules are enforced before data
    /// is persisted.
    /// </summary>
    public class BaseInfo : IBaseInfo
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Article> _articleRepository;
        private readonly IGenericRepository<Tag> _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCategoryDto> _createCategoryValidator;
        private readonly IValidator<UpdateCategoryDto> _updateCategoryValidator;

        public BaseInfo(
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Article> articleRepository,
            IGenericRepository<Tag> tagRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateCategoryDto> createCategoryValidator,
            IValidator<UpdateCategoryDto> updateCategoryValidator)
        {
            _categoryRepository = categoryRepository;
            _articleRepository = articleRepository;
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
            _createCategoryValidator = createCategoryValidator;
            _updateCategoryValidator = updateCategoryValidator;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(CreateCategoryDto dto)
        {
            var title = NormalizeTitle(dto.Title);

            // Application-layer business rules: validate the normalized input
            // before it is allowed to reach the data store.
            await EnsureCreateValidAsync(title);

            if (await TitleExistsAsync(title))
            {
                throw new DuplicateCategoryNameException(title);
            }

            var category = new Category
            {
                Title = title,
                Description = string.Empty,
                CreateDate = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangedAsync();
        }

        public async Task UpdateAsync(UpdateCategoryDto dto)
        {
            var title = NormalizeTitle(dto.Title);

            // Validate the normalized input with the update rules, so the same
            // data-quality requirements apply as on create.
            await EnsureUpdateValidAsync(dto.Id, title);

            // A rename must still respect uniqueness, but it has to ignore the
            // category being edited itself (renaming X to "X" is always legal).
            if (await TitleExistsExcludingAsync(title, dto.Id))
            {
                throw new DuplicateCategoryNameException(title);
            }

            // Load the real aggregate from the store; the request only carries
            // the fields the user may change.
            var category = await _categoryRepository.GetByIdAsync(dto.Id);
            if (category is null)
            {
                throw new CategoryNotFoundException(dto.Id);
            }

            category.Title = title;
            category.UpdateDate = DateTime.UtcNow;

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangedAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                throw new CategoryNotFoundException(id);
            }

            // Referential-integrity rule: because the store cascades deletes
            // from Category to Article, we refuse to delete a category that is
            // still referenced — otherwise all its articles would disappear
            // silently. This turns an implicit data-loss trap into an explicit,
            // communicated business rule.
            var hasArticles = await _articleRepository.AnyAsync(
                article => article.CategoryId == id);

            if (hasArticles)
            {
                throw new CategoryHasArticlesException(id, category.Title);
            }

            _categoryRepository.Delete(category);
            await _unitOfWork.SaveChangedAsync();
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task DeleteTagAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag is null)
            {
                throw new TagNotFoundException(id);
            }

            // Note on referential integrity: the Tag -> ArticleTag relationship
            // cascades, but ArticleTag is a pure junction table. Deleting a tag
            // therefore only removes the tag's link rows — the articles that
            // used the tag are never deleted, so no data-loss guard is needed.
            _tagRepository.Delete(tag);
            await _unitOfWork.SaveChangedAsync();
        }

        public async Task<bool> TitleExistsAsync(string title)
        {
            var normalizedTitle = NormalizeTitle(title);

            var categories = await _categoryRepository.GetAllAsync();

            return categories.Any(x =>
                string.Equals(
                    NormalizeTitle(x.Title),
                    normalizedTitle,
                    StringComparison.OrdinalIgnoreCase));
        }

        #region Private Methods

        /// <summary>
        /// Validates a title for the create operation and raises a
        /// <see cref="AppValidationException"/> when it is unacceptable.
        /// </summary>
        private async Task EnsureCreateValidAsync(string title)
        {
            var validationResult = await _createCategoryValidator.ValidateAsync(
                new CreateCategoryDto { Title = title });

            ThrowIfInvalid(validationResult);
        }

        /// <summary>
        /// Validates an id/title pair for the update operation and raises a
        /// <see cref="AppValidationException"/> when it is unacceptable.
        /// </summary>
        private async Task EnsureUpdateValidAsync(int id, string title)
        {
            var validationResult = await _updateCategoryValidator.ValidateAsync(
                new UpdateCategoryDto { Id = id, Title = title });

            ThrowIfInvalid(validationResult);
        }

        /// <summary>
        /// Shared helper that turns a failed validation into the application's
        /// canonical <see cref="AppValidationException"/>.
        /// </summary>
        private static void ThrowIfInvalid(
            FluentValidation.Results.ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors);
            }
        }

        /// <summary>
        /// Returns whether any category other than the one being edited already
        /// uses the given (normalized) title.
        /// </summary>
        private async Task<bool> TitleExistsExcludingAsync(string title, int excludingId)
        {
            var normalizedTitle = NormalizeTitle(title);

            var categories = await _categoryRepository.GetAllAsync();

            return categories.Any(x =>
                x.Id != excludingId &&
                string.Equals(
                    NormalizeTitle(x.Title),
                    normalizedTitle,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizeTitle(string? title)
        {
            return title?.Trim() ?? string.Empty;
        }

        #endregion
    }
}
