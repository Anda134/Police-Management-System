using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.DTOs.Conferences;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Controllers
{
    /// <summary>Manages conference operations (REQ-33 to REQ-48).</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConferenceController : ControllerBase
    {
        private readonly IConferenceService _service;

        /// <summary>Initializes a new instance of ConferenceController.</summary>
        /// <param name="service">The conference service.</param>
        public ConferenceController(IConferenceService service)
        {
            _service = service;
        }

        /// <summary>Returns all conferences.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Returns a single conference by ID.</summary>
        /// <param name="id">The conference ID.</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var conference = await _service.GetByIdAsync(id);
            return conference is null ? NotFound() : Ok(conference);
        }

        /// <summary>Creates a new conference request (REQ-41 to REQ-45).</summary>
        /// <param name="request">Conference creation data.</param>
        [HttpPost]
        [Authorize(Roles = "Admin,ChiefInspector,StationHead,Agent")]
        public async Task<IActionResult> Create([FromBody] CreateConferenceRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest("Reason is required.");

            if (string.IsNullOrWhiteSpace(request.Callsign))
                return BadRequest("Callsign is required.");

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

        /// <summary>Starts a scheduled conference (REQ-48).</summary>
        /// <param name="id">The conference ID.</param>
        [HttpPatch("{id:int}/start")]
        [Authorize(Roles = "Admin,ChiefInspector")]
        public async Task<IActionResult> Start(int id)
        {
            var started = await _service.StartConferenceAsync(id);
            return started ? NoContent() : NotFound();
        }

        /// <summary>Adds participants to a conference (REQ-35).</summary>
        /// <param name="id">The conference ID.</param>
        /// <param name="participantIds">List of agent IDs to add.</param>
        [HttpPatch("{id:int}/participants")]
        [Authorize(Roles = "Admin,ChiefInspector,StationHead")]
        public async Task<IActionResult> AddParticipants(int id, [FromBody] List<int> participantIds)
        {
            if (participantIds is null || participantIds.Count == 0)
                return BadRequest("At least one participant is required.");

            var result = await _service.AddParticipantsAsync(id, participantIds);
            return result ? NoContent() : NotFound();
        }

        /// <summary>Deletes a conference. Admin and ChiefInspector only.</summary>
        /// <param name="id">The conference ID.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,ChiefInspector")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}