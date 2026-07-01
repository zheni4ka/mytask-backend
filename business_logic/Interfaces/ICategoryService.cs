using business_logic.DTOs;
using business_logic.DTOs.Assignment;
using System;
using System.Collections.Generic;
using System.Text;

namespace business_logic.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAll();
        Task<CategoryDTO> GetCategoryAsync(int id);
        Task InsertAsync(CreateCategoryModel assignment);
        Task UpdateAsync(EditCategoryModel assignment);
        Task DeleteAsync(int id);
    }
}
