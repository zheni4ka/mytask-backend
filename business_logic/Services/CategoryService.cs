using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
            await _categoryRepo.DeleteByIdAsync(id);
            await _categoryRepo.SaveAsync();
        }

        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {
            return _mapper.Map<IEnumerable<CategoryDTO>>(await _categoryRepo.GetAllAsync());
        }

        public async Task<CategoryDTO> GetCategoryAsync(int id)
        {
            return _mapper.Map<CategoryDTO>(await _categoryRepo.GetByIdAsync(id));
        }

        public async Task InsertAsync(CreateCategoryModel category)
        {
            await _categoryRepo.InsertAsync(_mapper.Map<Category>(category));
            await _categoryRepo.SaveAsync();
        }

        public async Task UpdateAsync(EditCategoryModel category)
        {
            _categoryRepo.Update(_mapper.Map<Category>(category));
            await _categoryRepo.SaveAsync();
        }

    }
}
