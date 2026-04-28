using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.Agents;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Services
{
    /// <summary>Handles CRUD operations for police agents (REQ-17 to REQ-24).</summary>
    public class AgentService : IAgentService
    {
        private readonly AppDbContext _context;

        /// <summary>Initializes a new instance of AgentService.</summary>
        /// <param name="context">The database context.</param>
        public AgentService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Returns all agents with station and superior info.</summary>
        public async Task<IEnumerable<AgentDto>> GetAllAsync()
        {
            return await _context.Agents
                .Include(a => a.PoliceStation)
                .Include(a => a.Superior)
                .Select(a => new AgentDto
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Badge = a.Badge,
                    Role = a.Role,
                    PoliceStationId = a.PoliceStationId,
                    PoliceStationName = a.PoliceStation.Name,
                    SuperiorId = a.SuperiorId,
                    SuperiorName = a.Superior != null
                        ? a.Superior.FirstName + " " + a.Superior.LastName
                        : null
                })
                .ToListAsync();
        }

        /// <summary>Returns a single agent by ID.</summary>
        /// <param name="id">The agent ID.</param>
        public async Task<AgentDto?> GetByIdAsync(int id)
        {
            var agent = await _context.Agents
                .Include(a => a.PoliceStation)
                .Include(a => a.Superior)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agent is null) return null;

            return new AgentDto
            {
                Id = agent.Id,
                FirstName = agent.FirstName,
                LastName = agent.LastName,
                Badge = agent.Badge,
                Role = agent.Role,
                PoliceStationId = agent.PoliceStationId,
                PoliceStationName = agent.PoliceStation.Name,
                SuperiorId = agent.SuperiorId,
                SuperiorName = agent.Superior != null
                    ? agent.Superior.FirstName + " " + agent.Superior.LastName
                    : null
            };
        }

        /// <summary>Creates a new agent.</summary>
        /// <param name="request">Agent creation data.</param>
        public async Task<AgentDto> CreateAsync(CreateAgentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException("First name is required.");

            if (string.IsNullOrWhiteSpace(request.Badge))
                throw new ArgumentException("Badge is required.");

            var stationExists = await _context.PoliceStations
                .AnyAsync(ps => ps.Id == request.PoliceStationId);
            if (!stationExists)
                throw new ArgumentException("Police station not found.");

            var agent = new Agent
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Badge = request.Badge,
                Role = request.Role,
                PoliceStationId = request.PoliceStationId,
                SuperiorId = request.SuperiorId
            };

            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(agent.Id))!;
        }

        /// <summary>Updates an existing agent.</summary>
        /// <param name="id">The agent ID.</param>
        /// <param name="request">Updated agent data.</param>
        public async Task<bool> UpdateAsync(int id, UpdateAgentRequest request)
        {
            var agent = await _context.Agents.FindAsync(id);
            if (agent is null) return false;

            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException("First name is required.");

            agent.FirstName = request.FirstName;
            agent.LastName = request.LastName;
            agent.Badge = request.Badge;
            agent.Role = request.Role;
            agent.PoliceStationId = request.PoliceStationId;
            agent.SuperiorId = request.SuperiorId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Deletes an agent by ID.</summary>
        /// <param name="id">The agent ID.</param>
        public async Task<bool> DeleteAsync(int id)
        {
            var agent = await _context.Agents.FindAsync(id);
            if (agent is null) return false;

            _context.Agents.Remove(agent);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Assigns or removes a superior for an agent (REQ-21, REQ-23).</summary>
        /// <param name="agentId">The agent ID.</param>
        /// <param name="superiorId">The superior ID, or null to remove.</param>
        public async Task<bool> AssignSuperiorAsync(int agentId, int? superiorId)
        {
            var agent = await _context.Agents.FindAsync(agentId);
            if (agent is null) return false;

            if (superiorId.HasValue)
            {
                // REQ-23: prevent circular subordination
                if (superiorId.Value == agentId)
                    throw new InvalidOperationException("An agent cannot be their own superior.");

                var superiorExists = await _context.Agents
                    .AnyAsync(a => a.Id == superiorId.Value);
                if (!superiorExists)
                    throw new ArgumentException("Superior agent not found.");
            }

            agent.SuperiorId = superiorId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}