using Ardalis.Specification;
using business_logic.DTOs;
using business_logic.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace mytask_backend.Controllers
{
    [Authorize]
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var validationResult = await _createStepValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _stepService.InsertAsync(model, userId);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _stepService.DeleteAsync(id, userId);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] EditStepModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var validationResult = await _editStepValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _stepService.UpdateAsync(model, userId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _stepService.GetAll(userId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var step = await _stepService.GetStepAsync(id, userId);
            return Ok(step);
        }

        [HttpGet("by-assignment/{taskId:int}")]
        public async Task<IActionResult> GetByAssignmentId(int taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _stepService.GetByAssigmentId(taskId, userId));
        }


    }
}
