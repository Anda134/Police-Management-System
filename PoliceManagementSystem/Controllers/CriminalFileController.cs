using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.DTOs.CriminalFiles;
using PoliceManagementSystem.Services.Interfaces;
using System.Security.Claims;

namespace PoliceManagementSystem.Controllers
{
    /// <summary>Manages criminal file operations (REQ-49 to REQ-64).</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CriminalFileController : ControllerBase
    {
        private readonly ICriminalFileService _service;

        /// <summary>Initializes a new instance of CriminalFileController.</summary>
        /// <param name="service">The criminal file service.</param>
        public CriminalFileController(ICriminalFileService service)
        {
            _service = service;
        }

        /// <summary>Returns all criminal files, with optional filters (REQ-57, REQ-58, REQ-59).</summary>
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

        /// <summary>Returns a single criminal file by ID (REQ-51).</summary>
        /// <param name="id">The file ID.</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var file = await _service.GetByIdAsync(id);
            return file is null ? NotFound() : Ok(file);
        }

        /// <summary>Creates a new criminal file (REQ-49, REQ-50).</summary>
        /// <param name="request">File creation data.</param>
        [HttpPost]
        [Authorize(Roles = "Admin,ChiefInspector,StationHead,Agent")]
        public async Task<IActionResult> Create([FromBody] CreateCriminalFileRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title is required.");

            try
            {
                var created = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Updates an existing criminal file (REQ-52, REQ-53).</summary>
        /// <param name="id">The file ID.</param>
        /// <param name="request">Updated file data.</param>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,ChiefInspector,StationHead,Agent")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCriminalFileRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title is required.");

            try
            {
                var updated = await _service.UpdateAsync(id, request);
                return updated ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Transfers a file to another station (REQ-60, REQ-61, REQ-62, REQ-63, REQ-64).</summary>
        /// <param name="id">The file ID.</param>
        /// <param name="newStationId">The destination station ID.</param>
        [HttpPatch("{id:int}/transfer/{newStationId:int}")]
        [Authorize(Roles = "Admin,ChiefInspector,StationHead")]
        public async Task<IActionResult> Transfer(int id, int newStationId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            try
            {
                var result = await _service.TransferAsync(id, newStationId, userId);
                return result ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Deletes a criminal file. Admin and ChiefInspector only (REQ-55).</summary>
        /// <param name="id">The file ID.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,ChiefInspector")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}