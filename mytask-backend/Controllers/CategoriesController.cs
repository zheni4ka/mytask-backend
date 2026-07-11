using Ardalis.Specification;
using Core.DTOs;
using Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace mytask_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<CreateCategoryModel> _createCategoryValidator;
        private readonly IValidator<EditCategoryModel> _editCategoryValidator;

        public CategoriesController(ICategoryService categoryService, IValidator<CreateCategoryModel> createCategoryValidator, IValidator<EditCategoryModel> editCategoryValidator)
        {
            this._categoryService = categoryService;
            this._createCategoryValidator = createCategoryValidator;
            this._editCategoryValidator = editCategoryValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var validationResult = await _createCategoryValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _categoryService.InsertAsync(model, userId);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit(EditCategoryModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var validationResult = await _editCategoryValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _categoryService.UpdateAsync(model, userId);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _categoryService.DeleteAsync(id, userId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _categoryService.GetAll(userId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _categoryService.GetCategoryAsync(id, userId));
        }
    }
}
