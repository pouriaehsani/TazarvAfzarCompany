using Company.Application.DTOs.Category;
using Company.Application.Exceptions;
using Company.Application.Interfaces;
using Company.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Services
{
    public class BaseInfo : IBaseInfo
    {
        private readonly IGenericRepository<Category> _cat;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCategoryDto> _createCategoryValidator;

        public BaseInfo(
            IGenericRepository<Category> genericRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateCategoryDto> createCategoryValidator)
        {
            _cat = genericRepository;
            _unitOfWork = unitOfWork;
            _createCategoryValidator = createCategoryValidator;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _cat.GetAllAsync();
        }

        public async Task CreateAsync(CreateCategoryDto dto)
        {
            var title = NormalizeTitle(dto.Title);

            // Application-layer business rules: validate the normalized input.
            var validationResult = await _createCategoryValidator.ValidateAsync(
                new CreateCategoryDto { Title = title });

            if (!validationResult.IsValid)
            {
                throw new Company.Application.Exceptions.ValidationException(
                    validationResult.Errors);
            }

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

            await _cat.AddAsync(category);
            await _unitOfWork.SaveChangedAsync();
        }

        public async Task<bool> TitleExistsAsync(string title)
        {
            var normalizedTitle = NormalizeTitle(title);

            var categories = await _cat.GetAllAsync();

            return categories.Any(x =>
                string.Equals(
                    NormalizeTitle(x.Title),
                    normalizedTitle,
                    StringComparison.OrdinalIgnoreCase));
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Category cat)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private static string NormalizeTitle(string? title)
        {
            return title?.Trim() ?? string.Empty;
        }

        #endregion
    }
}
