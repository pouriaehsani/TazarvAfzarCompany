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
        }
        public async Task CreateAsync(CreateArticleDto createDto)
        {
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

        public async Task UpdateAsync(UpdateArticleDto updateDto)
        {
            var article = _mapper.Map<Article>(updateDto);
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