using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.DTOs.AgentTransfers;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Controllers
{
    /// <summary>Manages agent transfer operations (REQ-65 to REQ-72).</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgentTransferController : ControllerBase
    {
        private readonly IAgentTransferService _service;

        /// <summary>Initializes a new instance of AgentTransferController.</summary>
        /// <param name="service">The agent transfer service.</param>
        public AgentTransferController(IAgentTransferService service)
        {
            _service = service;
        }

        /// <summary>Returns all agent transfers (REQ-72).</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Returns a single agent transfer by ID.</summary>
        /// <param name="id">The transfer ID.</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transfer = await _service.GetByIdAsync(id);
            return transfer is null ? NotFound() : Ok(transfer);
        }

        /// <summary>Initiates a new agent transfer (REQ-65, REQ-66, REQ-68, REQ-69, REQ-71).</summary>
        /// <param name="request">Transfer creation data.</param>
        [HttpPost]
        [Authorize(Roles = "Admin,ChiefInspector,StationHead")]
        public async Task<IActionResult> Create([FromBody] CreateAgentTransferRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest("Reason is required.");

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

        /// <summary>Approves a permanent transfer. ChiefInspector only (REQ-67).</summary>
        /// <param name="id">The transfer ID.</param>
        [HttpPatch("{id:int}/approve")]
        [Authorize(Roles = "Admin,ChiefInspector")]
        public async Task<IActionResult> Approve(int id)
        {
            var approved = await _service.ApproveAsync(id);
            return approved ? NoContent() : NotFound();
        }

        /// <summary>Deletes a transfer. Admin only.</summary>
        /// <param name="id">The transfer ID.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}