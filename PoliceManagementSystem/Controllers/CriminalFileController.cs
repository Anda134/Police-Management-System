using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CriminalFileController : ControllerBase
    {
        private readonly ICriminalFileService _service;

        public CriminalFileController(ICriminalFileService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? title,
            [FromQuery] string? category,
            [FromQuery] int? agentId)
        {
            if (title != null || category != null || agentId.HasValue)
                return Ok(await _service.SearchAsync(title, category, agentId));

            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var file = await _service.GetByIdAsync(id);
            return file == null ? NotFound() : Ok(file);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CriminalFile file)
        {
            var created = await _service.CreateAsync(file);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CriminalFile file)
        {
            var updated = await _service.UpdateAsync(id, file);
            return updated ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}/transfer/{newStationId:int}")]
        public async Task<IActionResult> Transfer(int id, int newStationId)
        {
            var result = await _service.TransferAsync(id, newStationId);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}