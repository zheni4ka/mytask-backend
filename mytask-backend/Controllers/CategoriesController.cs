using business_logic.DTOs;
using business_logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace mytask_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryModel model)
        {
            await _categoryService.InsertAsync(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit(EditCategoryModel model)
        {
            await _categoryService.UpdateAsync(model);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _categoryService.GetAll());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _categoryService.GetCategoryAsync(id));
        }

        [HttpGet("latest/{id:int}")]
        public async Task<IActionResult> GetByLatestAssignments(int id)
        {
            return Ok(await _categoryService.GetByLatestAssignments(id));
        }

        [HttpGet("with-assignments")]
        public async Task<IActionResult> GetCategoriesWithAssignments()
        {
            return Ok(await _categoryService.GetCategoriesWithAssignments());
        }

        [HttpGet("empty")]
        public async Task<IActionResult> GetEmptyCategories()
        {
            return Ok(await _categoryService.GetEmptyCategories());
        }

    }
}
