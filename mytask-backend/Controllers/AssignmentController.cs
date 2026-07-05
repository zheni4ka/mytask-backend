using business_logic.DTOs;
using business_logic.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace mytask_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IValidator<CreateAssignmentModel> _createValidator;
        private readonly IValidator<EditAssignmentModel> _editValidator;

        public AssignmentController(IAssignmentService assignmentService, IValidator<EditAssignmentModel> editValidator, IValidator<CreateAssignmentModel> createValidator)
        {
            _assignmentService = assignmentService;
            _editValidator = editValidator;
            _createValidator = createValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssignmentModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Error of user authentication");
            }

            var validationResult = await _createValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _assignmentService.InsertAsync(model, userId);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _assignmentService.DeleteAsync(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] EditAssignmentModel model) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var validationResult = await _editValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _assignmentService.UpdateAsync(model, userId);
            return Ok();
        }

        [HttpGet("all")]
        public async Task<ActionResult<PagedResult<AssignmentDTO>>> GetAssignments([FromQuery] PageParameters parameters)
        {
            var result = await _assignmentService.GetPagedAssignmentsAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _assignmentService.GetAssignmentAsync(id));
        }

        [HttpGet("latest/{id:int}")]
        public async Task<IActionResult> GetLatestByCategoryId(int id)
        {
            return Ok(await _assignmentService.GetLatestByCategoryId(id));
        }

        [HttpGet("by-category/{id:int}")]
        public async Task<IActionResult> GetByCategoryId(int id)
        {
            return Ok(await _assignmentService.GetByCategoryId(id));
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueAssignments()
        {
            return Ok(await _assignmentService.GetOverdueAssignments());
        }

        [HttpGet("upcoming/{daysAhead:int}")]
        public async Task<IActionResult> GetUpcomingAssignments(int daysAhead)
        {
            return Ok(await _assignmentService.GetUpcomingAssignments(daysAhead));
        }

    }
}
