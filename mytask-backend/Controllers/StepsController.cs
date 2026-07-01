using business_logic.DTOs;
using business_logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace mytask_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StepsController : Controller
    {
        private readonly IStepService _stepService;

        public StepsController(IStepService stepService)
        {
            _stepService = stepService;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStepModel model)
        {
            await _stepService.InsertAsync(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            await _stepService.DeleteAsync(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] EditStepModel model)
        {
            await _stepService.UpdateAsync(model);
            return Ok();
        }
    }
}
