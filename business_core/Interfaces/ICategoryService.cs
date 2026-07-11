using Core.DTOs;

namespace Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAll(string userId);
        Task<CategoryDTO> GetCategoryAsync(int id, string userId);
        Task InsertAsync(CreateCategoryModel assignment, string userId);
        Task UpdateAsync(EditCategoryModel assignment, string userId);
        Task DeleteAsync(int id, string userId);
        Task<IEnumerable<CategoryDTO>> GetCategoriesWithAssignments(string userId);
        Task<IEnumerable<CategoryDTO>> GetEmptyCategories(string userId);
        Task CreateDefaultCategoriesAsync(string userId);
    }
}
