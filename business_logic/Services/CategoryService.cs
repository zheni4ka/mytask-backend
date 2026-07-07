using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Interfaces;
using business_logic.Specifications;
using static business_logic.Specifications.AssignmentSpecs;

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

        public async Task DeleteAsync(int id, string userId)
        {
            var category = await _categoryRepo.GetItemBySpecAsync(new CategorySpecs.ById(id, userId));
            if (category == null)
                throw new KeyNotFoundException("Category not found");

            var hasAssignments = await _categoryRepo.GetListBySpecAsync(new CategorySpecs.EmptyCategories(userId));
            if (hasAssignments.Any(x => x.Id == id))
            {
                throw new InvalidOperationException("Unable to delete category because it contains assignments.");
            }

            await _categoryRepo.DeleteByIdAsync(id);
            await _categoryRepo.SaveAsync();
        }

        public async Task<IEnumerable<CategoryDTO>> GetAll(string userId)
        {
            var categories = await _categoryRepo.GetListBySpecAsync(new CategorySpecs.All(userId));
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryAsync(int id, string userId)
        {
            var category = await _categoryRepo.GetItemBySpecAsync(new CategorySpecs.ById(id, userId));
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

        public async Task<IEnumerable<CategoryDTO>> GetByLatestAssignments(int id, string userId)
        {
            var categories = await _categoryRepo.GetListBySpecAsync(new CategorySpecs.ByLatestAssignments(id, userId));
            if(!categories.Any())
                throw new KeyNotFoundException("No categories found for the given assignment");
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesWithAssignments(string userId)
        {
            var categories = await _categoryRepo.GetListBySpecAsync(new CategorySpecs.CategoriesWithActiveTasks(userId));

            if(!categories.Any()) throw new KeyNotFoundException("No categories with active tasks found"); 

            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<IEnumerable<CategoryDTO>> GetEmptyCategories(string userId)
        {
            var categories = await _categoryRepo.GetListBySpecAsync(new CategorySpecs.EmptyCategories(userId));

            if(!categories.Any()) throw new KeyNotFoundException("No empty categories found");

            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task CreateDefaultCategoriesAsync(string userId)
        {
            var defaultCategoryNames = new[] 
            { 
                "Робота", 
                "Особисте", 
                "Здоров'я", 
                "Навчання",
                "Спорт"
            };

            foreach (var categoryName in defaultCategoryNames)
            {
                var category = new Category
                {
                    Name = categoryName,
                    UserId = userId
                };

                await _categoryRepo.InsertAsync(category);
            }

            await _categoryRepo.SaveAsync();
        }

    }
}
