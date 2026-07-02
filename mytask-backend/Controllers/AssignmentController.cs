using business_logic.DTOs;
using business_logic.DTOs.Assignment;
using business_logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace mytask_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : Controller
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssignmentModel model)
        {
            await _assignmentService.InsertAsync(model);
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
            await _assignmentService.UpdateAsync(model);
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
