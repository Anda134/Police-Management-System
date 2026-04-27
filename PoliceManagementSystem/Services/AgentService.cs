using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services
{
    public class AgentService : IAgentService
    {
        private readonly AppDbContext _context;

        public AgentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Agent>> GetAllAsync()
        {
            return await _context.Agents
                .Include(a => a.PoliceStation)
                .Include(a => a.Superior)
                .Include(a => a.Subordinates)
                .ToListAsync();
        }

        public async Task<Agent?> GetByIdAsync(int id)
        {
            return await _context.Agents
                .Include(a => a.PoliceStation)
                .Include(a => a.Superior)
                .Include(a => a.Subordinates)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Agent> CreateAsync(Agent agent)
        {
            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();
            return agent;
        }

        public async Task<bool> UpdateAsync(int id, Agent agent)
        {
            var existing = await _context.Agents.FindAsync(id);
            if (existing == null) return false;

            existing.FirstName = agent.FirstName;
            existing.LastName = agent.LastName;
            existing.Badge = agent.Badge;
            existing.Role = agent.Role;
            existing.PoliceStationId = agent.PoliceStationId;
            existing.SuperiorId = agent.SuperiorId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Agents.FindAsync(id);
            if (existing == null) return false;

            _context.Agents.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignSuperiorAsync(int agentId, int? superiorId)
        {
            var agent = await _context.Agents.FindAsync(agentId);
            if (agent == null) return false;

            if (superiorId.HasValue)
            {
                var superior = await _context.Agents.FindAsync(superiorId.Value);
                if (superior == null || superior.Id == agentId) return false;
            }

            agent.SuperiorId = superiorId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}