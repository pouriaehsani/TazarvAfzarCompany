using Company.Application.DTOs.Article;
using Company.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Interfaces
{
    public interface IArticleService
    {
        Task<List<Article>> GetAllAsync();

        Task<Article?> GetByIdAsync(int id);

        /// <summary>Loads an article together with its tags, for editing.</summary>
        Task<Article?> GetByIdWithTagsAsync(int id);

        Task CreateAsync(CreateArticleDto CreateDto);

        Task UpdateAsync(UpdateArticleDto UpdateDto);

        Task DeleteAsync(int id);

        Task<string> UploadContentImageAsync(IFormFile file);
    }
}
