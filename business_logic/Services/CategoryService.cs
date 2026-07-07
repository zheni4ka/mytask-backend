using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Interfaces;
using business_logic.Specifications;

namespace business_logic.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryService(IRepository<Category> categoryRepo, IMapper mapper)
        {
            this._categoryRepo = categoryRepo;
            this._mapper = mapper;
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepo.GetItemBySpecAsync(new CategorySpecs.ById(id));
            if (category == null)
                throw new KeyNotFoundException("Category not found");
            await _categoryRepo.DeleteByIdAsync(id);
            await _categoryRepo.SaveAsync();
        }

        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {
            return _mapper.Map<IEnumerable<CategoryDTO>>(await _categoryRepo.GetAllAsync());
        }

        public async Task<CategoryDTO> GetCategoryAsync(int id)
        {
            var category = await _categoryRepo.GetItemBySpecAsync(new CategorySpecs.ById(id));
            if (category == null)
                throw new KeyNotFoundException("Category not found");
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task InsertAsync(CreateCategoryModel category, string userId)
        {
            var categoryEntity = _mapper.Map<Category>(category);
            categoryEntity.UserId = userId;
            await _categoryRepo.InsertAsync(categoryEntity);
            await _categoryRepo.SaveAsync();
        }

        public async Task UpdateAsync(EditCategoryModel category, string userId)
        {
            var categoryEntity = _mapper.Map<Category>(category);
            categoryEntity.UserId = userId;
            _categoryRepo.Update(categoryEntity);
            await _categoryRepo.SaveAsync();
        }

        public async Task<IEnumerable<CategoryDTO>> GetByLatestAssignments(int id)
        {
            var categories = await _categoryRepo.GetListBySpecAsync(new CategorySpecs.ByLatestAssignments(id));
            if(!categories.Any())
                throw new KeyNotFoundException("No categories found for the given assignment");
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesWithAssignments()
        {
            var categories = await _categoryRepo.GetListBySpecAsync(new CategorySpecs.CategoriesWithActiveTasks());

            if(!categories.Any()) throw new KeyNotFoundException("No categories with active tasks found"); 

            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<IEnumerable<CategoryDTO>> GetEmptyCategories()
        {
            var categories = await _categoryRepo.GetListBySpecAsync(new CategorySpecs.EmptyCategories());

            if(!categories.Any()) throw new KeyNotFoundException("No empty categories found");

            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

    }
}
