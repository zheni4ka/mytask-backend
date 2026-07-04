using Ardalis.Specification;
using business_logic.DTOs;
using business_logic.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace mytask_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StepsController : Controller
    {
        private readonly IStepService _stepService;
        private readonly IValidator<CreateStepModel> _createStepValidator;
        private readonly IValidator<EditStepModel> _editStepValidator;
        public StepsController(IStepService stepService, IValidator<CreateStepModel> createStepValidator, IValidator<EditStepModel> editStepValidator)
        {
            _stepService = stepService;
            _createStepValidator = createStepValidator;
            _editStepValidator = editStepValidator;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStepModel model)
        {
            var validationResult = await _createStepValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _stepService.InsertAsync(model);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _stepService.DeleteAsync(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] EditStepModel model)
        {
            var validationResult = await _editStepValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _stepService.UpdateAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _stepService.GetAll());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var step = await _stepService.GetStepAsync(id);
            if (step == null)
            {
                return NotFound();
            }
            return Ok(step);
        }



    }
}
