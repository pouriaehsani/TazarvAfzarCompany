using Company.Application.DTOs.Category;
using Company.Domain.Entities;
using System.Threading.Tasks;

namespace Company.Application.Interfaces
{
    /// <summary>
    /// Application-layer contract for managing base information such as
    /// categories.
    ///
    /// The interface intentionally exposes DTOs (not domain entities) for
    /// write operations so that the web layer never has to reason about
    /// persistence shapes, honouring the dependency rule of Clean
    /// Architecture.
    /// </summary>
    public interface IBaseInfo
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task CreateAsync(CreateCategoryDto dto);

        Task UpdateAsync(UpdateCategoryDto dto);

        Task DeleteAsync(int id);
    }
}
