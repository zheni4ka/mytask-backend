using business_logic.DTOs;
using business_logic.Specifications;

namespace business_logic.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAll();
        Task<CategoryDTO> GetCategoryAsync(int id);
        Task InsertAsync(CreateCategoryModel assignment);
        Task UpdateAsync(EditCategoryModel assignment);
        Task DeleteAsync(int id);
        Task<IEnumerable<CategoryDTO>> GetByLatestAssignments(int id);
        Task<IEnumerable<CategoryDTO>> GetCategoriesWithAssignments();
        Task<IEnumerable<CategoryDTO>> GetEmptyCategories();
    }
}
