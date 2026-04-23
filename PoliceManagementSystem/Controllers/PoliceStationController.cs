using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoliceStationController : ControllerBase
    {
        private readonly IPoliceStationService _service;

        public PoliceStationController(IPoliceStationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var station = await _service.GetByIdAsync(id);
            return station == null ? NotFound() : Ok(station);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PoliceStation station)
        {
            var created = await _service.CreateAsync(station);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PoliceStation station)
        {
            var updated = await _service.UpdateAsync(id, station);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}