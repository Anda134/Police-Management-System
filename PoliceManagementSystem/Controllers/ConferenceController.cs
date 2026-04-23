using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Controllers
{
    public record CreateConferenceRequest(
        string Reason,
        string Callsign,
        int Priority,
        DateTime ScheduledAt,
        int OrganizerId,
        List<int>? ParticipantIds);

    public record AddParticipantsRequest(List<int> ParticipantIds);

    [ApiController]
    [Route("api/[controller]")]
    public class ConferenceController : ControllerBase
    {
        private readonly IConferenceService _service;

        public ConferenceController(IConferenceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var conference = await _service.GetByIdAsync(id);
            return conference == null ? NotFound() : Ok(conference);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateConferenceRequest request)
        {
            var conference = new Conference
            {
                Reason = request.Reason,
                Callsign = request.Callsign,
                Priority = request.Priority,
                ScheduledAt = request.ScheduledAt,
                OrganizerId = request.OrganizerId
            };

            var created = await _service.CreateAsync(conference, request.ParticipantIds);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPatch("{id:int}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var started = await _service.StartConferenceAsync(id);
            return started ? NoContent() : NotFound();
        }

        [HttpPatch("{id:int}/participants")]
        public async Task<IActionResult> AddParticipants(int id, [FromBody] AddParticipantsRequest request)
        {
            var result = await _service.AddParticipantsAsync(id, request.ParticipantIds);
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