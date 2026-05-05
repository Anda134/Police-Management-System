using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Repositories.Interfaces;

namespace PoliceManagementSystem.Repositories
{
    public class AgentTransferRepository : Repository<AgentTransfer>, IAgentTransferRepository
    {
        public AgentTransferRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AgentTransfer>> GetTransfersWithDetailsAsync()
        {
            return await _context.AgentTransfers
                .Include(transfer => transfer.Agent)
                .Include(transfer => transfer.FromStation)
                .Include(transfer => transfer.ToStation)
                .ToListAsync();
        }

        public async Task<AgentTransfer?> GetTransferWithDetailsByIdAsync(int id)
        {
            return await _context.AgentTransfers
                .Include(transfer => transfer.Agent)
                .Include(transfer => transfer.FromStation)
                .Include(transfer => transfer.ToStation)
                .FirstOrDefaultAsync(transfer => transfer.Id == id);
        }
    }
}