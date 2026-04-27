using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentTransferController : ControllerBase
    {
        private readonly IAgentTransferService _service;

        public AgentTransferController(IAgentTransferService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transfer = await _service.GetByIdAsync(id);
            return transfer == null ? NotFound() : Ok(transfer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AgentTransfer transfer)
        {
            var created = await _service.CreateAsync(transfer);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPatch("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var approved = await _service.ApproveAsync(id);
            return approved ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}