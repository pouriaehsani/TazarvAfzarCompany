using Company.Application.DTOs.Category;
using Company.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Company.Application.Interfaces
{
    /// <summary>
    /// Application-layer contract for managing base information on the BaseInfo
    /// screen: categories and tags.
    ///
    /// The interface intentionally exposes DTOs (not domain entities) for
    /// write operations so that the web layer never has to reason about
    /// persistence shapes, honouring the dependency rule of Clean
    /// Architecture.
    /// </summary>
    public interface IBaseInfo
    {
        // ---- Categories ----

        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task CreateAsync(CreateCategoryDto dto);

        Task UpdateAsync(UpdateCategoryDto dto);

        Task DeleteAsync(int id);

        // ---- Tags ----

        Task<List<Tag>> GetAllTagsAsync();

        Task DeleteTagAsync(int id);
    }
}
