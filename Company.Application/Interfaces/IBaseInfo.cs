using Company.Application.DTOs.Article;
using Company.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Application.Interfaces
{
    public interface IBaseInfo
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task CreateAsync(Category cat);

        Task UpdateAsync(Category cat);

        Task DeleteAsync(int id);
    }
}
