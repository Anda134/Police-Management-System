using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _service;

        public AgentController(IAgentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var agent = await _service.GetByIdAsync(id);
            return agent == null ? NotFound() : Ok(agent);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Agent agent)
        {
            var created = await _service.CreateAsync(agent);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Agent agent)
        {
            var updated = await _service.UpdateAsync(id, agent);
            return updated ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}/superior/{superiorId:int?}")]
        public async Task<IActionResult> AssignSuperior(int id, int? superiorId)
        {
            var result = await _service.AssignSuperiorAsync(id, superiorId);
            return result ? NoContent() : BadRequest();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}