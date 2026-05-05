using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Repositories.Interfaces;

namespace PoliceManagementSystem.Repositories
{
    public class AgentRepository
        : Repository<Agent>,
          IAgentRepository
    {
        public AgentRepository(
            AppDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<Agent>>
            GetAgentsWithHierarchyAsync()
        {
            return await _context.Agents
                .Include(a => a.PoliceStation)
                .Include(a => a.Superior)
                .Include(a => a.Subordinates)
                .ToListAsync();
        }

        public async Task<Agent?> GetAgentWithDetailsAsync(int id)
{
    return await _context.Agents
        .Include(a => a.PoliceStation)
        .Include(a => a.Superior)
        .Include(a => a.Subordinates)
        .FirstOrDefaultAsync(a => a.Id == id);
}
    }
}