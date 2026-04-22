using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services
{
    public class AgentTransferService : IAgentTransferService
    {
        private readonly AppDbContext _context;

        public AgentTransferService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AgentTransfer>> GetAllAsync()
        {
            return await _context.AgentTransfers
                .Include(t => t.Agent)
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .ToListAsync();
        }

        public async Task<AgentTransfer?> GetByIdAsync(int id)
        {
            return await _context.AgentTransfers
                .Include(t => t.Agent)
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<AgentTransfer> CreateAsync(AgentTransfer transfer)
        {
            _context.AgentTransfers.Add(transfer);
            await _context.SaveChangesAsync();
            return transfer;
        }

        public async Task<bool> ApproveAsync(int id)
        {
            var transfer = await _context.AgentTransfers.FindAsync(id);
            if (transfer == null) return false;

            transfer.IsApproved = true;

            var agent = await _context.Agents.FindAsync(transfer.AgentId);
            if (agent == null) return false;

            agent.PoliceStationId = transfer.ToStationId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transfer = await _context.AgentTransfers.FindAsync(id);
            if (transfer == null) return false;

            _context.AgentTransfers.Remove(transfer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}