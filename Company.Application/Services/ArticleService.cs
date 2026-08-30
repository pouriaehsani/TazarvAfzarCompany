using AutoMapper;
using Company.Application.DTOs.Article;
using Company.Application.Interfaces;
using Company.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IGenericRepository<Article> _ArticleRepository;
        private readonly IGenericRepository<Tag> _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly IValidator<CreateArticleDto> _createValidator;
        private readonly IValidator<UpdateArticleDto> _updateValidator;


        public ArticleService(
            IGenericRepository<Article> genericRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateArticleDto> createValidator,
            IValidator<UpdateArticleDto> updateValidator,
            IFileStorageService fileStorage,
            IGenericRepository<Tag> tagRepository)
        {
            _ArticleRepository = genericRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _tagRepository = tagRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateArticleDto createDto)
        {
            // Application-layer validation before any business rules run.
            var validationResult = await _createValidator.ValidateAsync(createDto);

            if (!validationResult.IsValid)
            {
                throw new Company.Application.Exceptions.ValidationException(
                    validationResult.Errors);
            }

            var tagNames = PrepareTagNames(createDto.TagNames);
            var tags = await GetOrCreateTagsAsync(tagNames);
            var article = _mapper.Map<Article>(createDto);
            await SetArticleImageAsync(article, createDto.Image);
            article.CreateDate = DateTime.UtcNow;
            AddArticleTags(article, tags);
            await _ArticleRepository.AddAsync(article);
            await _unitOfWork.SaveChangedAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var article = await _ArticleRepository.GetByIdAsync(id);
            if (article is null)
            {
                return;
            }

            _ArticleRepository.Delete(article);
            await _unitOfWork.SaveChangedAsync();

        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await _ArticleRepository.GetAllAsync();
        }
        public Task<Article?> GetByIdAsync(int id)
        {
            return _ArticleRepository.GetByIdAsync(id);
        }

        public Task<Article?> GetByIdWithTagsAsync(int id)
        {
            return _ArticleRepository.GetByIdIncludingAsync(
                id, "ArticleTags.Tag");
        }

        public async Task UpdateAsync(UpdateArticleDto updateDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateDto);

            if (!validationResult.IsValid)
            {
                throw new Company.Application.Exceptions.ValidationException(
                    validationResult.Errors);
            }

            // Load the real aggregate (with its existing tag links) from the
            // store; the request only carries the fields the user may change.
            var article = await _ArticleRepository.GetByIdIncludingAsync(
                updateDto.Id, "ArticleTags");

            if (article is null)
            {
                throw new Company.Application.Exceptions.ArticleNotFoundException(
                    updateDto.Id);
            }

            article.Title = updateDto.Title;
            article.Description = updateDto.Description;
            article.Content = updateDto.Content;
            article.CategoryId = updateDto.CategoryId;
            article.UpdateDate = DateTime.UtcNow;

            // Only replace the stored image when a new one was uploaded.
            if (updateDto.Image is not null)
            {
                article.ImagePath = await _fileStorage.SaveAsync(
                    updateDto.Image,
                    "uploads/articles");
            }

            // Rebuild the article's tag links from the submitted tag names.
            var tagNames = PrepareTagNames(updateDto.TagNames);
            var tags = await GetOrCreateTagsAsync(tagNames);

            article.ArticleTags.Clear();
            AddArticleTags(article, tags);

            _ArticleRepository.Update(article);
            await _unitOfWork.SaveChangedAsync();
        }

        public async Task<string> UploadContentImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Image is required.", nameof(file));
            var imagePath = await _fileStorage.SaveAsync(file,"uploads/articles/content");
            return imagePath;
        }
        #region Private Methods

        private List<string> PrepareTagNames(
        List<string> tagNames)
        {
            return tagNames?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
                ?? new List<string>();
        }


        private async Task<List<Tag>> GetOrCreateTagsAsync(List<string> tagNames)
        {
            if (!tagNames.Any())
                return new List<Tag>();

            var existingTags = await _tagRepository.GetAllAsync();

            var matchedTags = existingTags.Where(x => tagNames
            .Contains(x.Title, StringComparer.OrdinalIgnoreCase)).ToList();

            var newTagNames = tagNames
                .Where(tagName =>
                    !matchedTags.Any(x =>
                        string.Equals(
                            x.Title,
                            tagName,
                            StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var newTags = newTagNames
                .Select(tagName => new Tag
                {
                    Title = tagName,
                    CreateDate = DateTime.UtcNow
                })
                .ToList();

            foreach (var tag in newTags)
            {
                await _tagRepository.AddAsync(tag);
            }

            return matchedTags
                .Concat(newTags)
                .ToList();
        }

        private async Task SetArticleImageAsync(Article article,IFormFile image)
        {
            var imagePath = await _fileStorage.SaveAsync(
                image,
                "uploads/articles");

            article.ImagePath = imagePath;
        }


        private void AddArticleTags(Article article,IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                article.ArticleTags.Add(new ArticleTag
                {
                    Article = article,
                    Tag = tag
                });
            }
        }
    }
}
#endregion