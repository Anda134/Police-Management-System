using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.AgentTransfers;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Services
{
    /// <summary>Handles operations for agent transfers (REQ-65 to REQ-72).</summary>
    public class AgentTransferService : IAgentTransferService
    {
        private readonly AppDbContext _context;

        /// <summary>Initializes a new instance of AgentTransferService.</summary>
        /// <param name="context">The database context.</param>
        public AgentTransferService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Returns all agent transfers with agent and station info.</summary>
        public async Task<IEnumerable<AgentTransferDto>> GetAllAsync()
        {
            return await _context.AgentTransfers
                .Include(at => at.Agent)
                .Include(at => at.FromStation)
                .Include(at => at.ToStation)
                .Select(at => new AgentTransferDto
                {
                    Id = at.Id,
                    AgentId = at.AgentId,
                    AgentName = at.Agent.FirstName + " " + at.Agent.LastName,
                    FromStationId = at.FromStationId,
                    FromStationName = at.FromStation.Name,
                    ToStationId = at.ToStationId,
                    ToStationName = at.ToStation.Name,
                    IsPermanent = at.IsPermanent,
                    StartDate = at.StartDate,
                    EndDate = at.EndDate,
                    Reason = at.Reason,
                    IsApproved = at.IsApproved
                })
                .ToListAsync();
        }

        /// <summary>Returns a single agent transfer by ID.</summary>
        /// <param name="id">The transfer ID.</param>
        public async Task<AgentTransferDto?> GetByIdAsync(int id)
        {
            var transfer = await _context.AgentTransfers
                .Include(at => at.Agent)
                .Include(at => at.FromStation)
                .Include(at => at.ToStation)
                .FirstOrDefaultAsync(at => at.Id == id);

            if (transfer is null) return null;

            return new AgentTransferDto
            {
                Id = transfer.Id,
                AgentId = transfer.AgentId,
                AgentName = transfer.Agent.FirstName + " " + transfer.Agent.LastName,
                FromStationId = transfer.FromStationId,
                FromStationName = transfer.FromStation.Name,
                ToStationId = transfer.ToStationId,
                ToStationName = transfer.ToStation.Name,
                IsPermanent = transfer.IsPermanent,
                StartDate = transfer.StartDate,
                EndDate = transfer.EndDate,
                Reason = transfer.Reason,
                IsApproved = transfer.IsApproved
            };
        }

        /// <summary>Initiates a new agent transfer (REQ-65, REQ-66, REQ-68, REQ-69, REQ-71).</summary>
        /// <param name="request">Transfer creation data.</param>
        public async Task<AgentTransferDto> CreateAsync(CreateAgentTransferRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new ArgumentException("Reason is required.");

            var agentExists = await _context.Agents
                .AnyAsync(a => a.Id == request.AgentId);
            if (!agentExists)
                throw new ArgumentException("Agent not found.");

            var fromStationExists = await _context.PoliceStations
                .AnyAsync(ps => ps.Id == request.FromStationId);
            if (!fromStationExists)
                throw new ArgumentException("Source station not found.");

            var toStationExists = await _context.PoliceStations
                .AnyAsync(ps => ps.Id == request.ToStationId);
            if (!toStationExists)
                throw new ArgumentException("Destination station not found.");

            if (request.FromStationId == request.ToStationId)
                throw new ArgumentException("Source and destination stations must be different.");

            if (!request.IsPermanent && request.EndDate is null)
                throw new ArgumentException("End date is required for temporary transfers.");

            if (!request.IsPermanent && request.EndDate <= request.StartDate)
                throw new ArgumentException("End date must be after start date.");

            var transfer = new AgentTransfer
            {
                AgentId = request.AgentId,
                FromStationId = request.FromStationId,
                ToStationId = request.ToStationId,
                IsPermanent = request.IsPermanent,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason,
                IsApproved = false
            };

            _context.AgentTransfers.Add(transfer);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(transfer.Id))!;
        }

        /// <summary>Approves a permanent transfer (REQ-67).</summary>
        /// <param name="id">The transfer ID.</param>
        public async Task<bool> ApproveAsync(int id)
        {
            var transfer = await _context.AgentTransfers
                .Include(at => at.Agent)
                .FirstOrDefaultAsync(at => at.Id == id);

            if (transfer is null) return false;

            transfer.IsApproved = true;

            // REQ-66: apply permanent transfer � update agent's station
            if (transfer.IsPermanent)
            {
                transfer.Agent.PoliceStationId = transfer.ToStationId;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Deletes a transfer by ID.</summary>
        /// <param name="id">The transfer ID.</param>
        public async Task<bool> DeleteAsync(int id)
        {
            var transfer = await _context.AgentTransfers.FindAsync(id);
            if (transfer is null) return false;

            _context.AgentTransfers.Remove(transfer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}