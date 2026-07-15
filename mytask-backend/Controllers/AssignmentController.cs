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
    public class AssignmentController : Controller
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IValidator<CreateAssignmentModel> _createValidator;
        private readonly IValidator<EditAssignmentModel> _editValidator;
        private readonly IGoogleCalendarService _calendarService;

        public AssignmentController(IAssignmentService assignmentService, 
            IValidator<EditAssignmentModel> editValidator, 
            IValidator<CreateAssignmentModel> createValidator,
            IGoogleCalendarService calendarService)
        {
            _assignmentService = assignmentService;
            _editValidator = editValidator;
            _createValidator = createValidator;
            _calendarService = calendarService;
        }

        [HttpPost("remove-from-calendar")]
        public async Task<IActionResult> RemoveFromCalendar([FromBody] GoogleEventDto eventDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "user is not identified" });
            }

            try
            {
                await _calendarService.DeleteEventAsync(eventDto, userId);
                return Ok(new { message = "Event is successfully removed from calendar" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add-to-calendar")]
        public async Task<IActionResult> AddToCalendar([FromBody] GoogleEventDto eventDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User is not identified" });
            }

            try
            {
                await _calendarService.CreateEventAsync(eventDto, userId);

                return Ok(new { message = "Event is successfully added to Google Calendar" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssignmentModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _assignmentService.DeleteAsync(id, userId);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _assignmentService.GetPagedAssignmentsAsync(parameters, userId);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _assignmentService.GetAssignmentAsync(id, userId));
        }

        

    }
}
