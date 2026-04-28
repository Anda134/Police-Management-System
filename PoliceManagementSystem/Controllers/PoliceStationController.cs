using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.DTOs.PoliceStations;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Controllers
{
    /// <summary>Manages police station CRUD operations (REQ-1 to REQ-8).</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PoliceStationController : ControllerBase
    {
        private readonly IPoliceStationService _service;

        /// <summary>Initializes a new instance of PoliceStationController.</summary>
        /// <param name="service">The police station service.</param>
        public PoliceStationController(IPoliceStationService service)
        {
            _service = service;
        }

        /// <summary>Returns all police stations.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Returns a single police station by ID.</summary>
        /// <param name="id">The station ID.</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var station = await _service.GetByIdAsync(id);
            return station is null ? NotFound() : Ok(station);
        }

        /// <summary>Creates a new police station. Admin and ChiefInspector only (REQ-1).</summary>
        /// <param name="request">Station creation data.</param>
        [HttpPost]
        [Authorize(Roles = "Admin,ChiefInspector")]
        public async Task<IActionResult> Create([FromBody] CreatePoliceStationRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Station name is required.");

            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Updates an existing police station. Admin and ChiefInspector only (REQ-3).</summary>
        /// <param name="id">The station ID.</param>
        /// <param name="request">Updated station data.</param>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,ChiefInspector")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePoliceStationRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Station name is required.");

            var updated = await _service.UpdateAsync(id, request);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>Deletes a police station. Admin only.</summary>
        /// <param name="id">The station ID.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}