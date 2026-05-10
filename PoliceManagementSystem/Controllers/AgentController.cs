using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.DTOs.Agents;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Controllers
{
    /// <summary>Manages police agent operations (REQ-17 to REQ-24).</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _service;

        /// <summary>Initializes a new instance of AgentController.</summary>
        /// <param name="service">The agent service.</param>
        public AgentController(IAgentService service)
        {
            _service = service;
        }

        /// <summary>Returns all agents.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Returns a single agent by ID.</summary>
        /// <param name="id">The agent ID.</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var agent = await _service.GetByIdAsync(id);
            return agent is null ? NotFound() : Ok(agent);
        }

        /// <summary>Creates a new agent. Admin and ChiefInspector only (REQ-17).</summary>
        /// <param name="request">Agent creation data.</param>
        [HttpPost]
        [Authorize(Roles = "Admin,ChiefInspector")]
        public async Task<IActionResult> Create([FromBody] CreateAgentRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.FirstName))
                return BadRequest("First name is required.");

            try
            {
                var created = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>Updates an existing agent. Admin and ChiefInspector only (REQ-19).</summary>
        /// <param name="id">The agent ID.</param>
        /// <param name="request">Updated agent data.</param>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,ChiefInspector")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAgentRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.FirstName))
                return BadRequest("First name is required.");

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

        /// <summary>Assigns or removes a superior for an agent (REQ-21, REQ-23).</summary>
        /// <param name="id">The agent ID.</param>
        /// <param name="superiorId">The superior ID, or null to remove.</param>
        [HttpPatch("{id:int}/superior/{superiorId:int?}")]
        [Authorize(Roles = "Admin,ChiefInspector,StationHead")]
        public async Task<IActionResult> AssignSuperior(int id, int? superiorId)
        {
            try
            {
                var result = await _service.AssignSuperiorAsync(id, superiorId);
                return result ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Deletes an agent. Admin only.</summary>
        /// <param name="id">The agent ID.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                return deleted ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}