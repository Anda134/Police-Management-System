using PoliceManagementSystem.DTOs.AgentTransfers;

namespace PoliceManagementSystem.Services.Interfaces
{
    /// <summary>Defines operations for managing agent transfers (REQ-65 to REQ-72).</summary>
    public interface IAgentTransferService
    {
        /// <summary>Returns all agent transfers.</summary>
        Task<IEnumerable<AgentTransferDto>> GetAllAsync();

        /// <summary>Returns a single agent transfer by ID.</summary>
        /// <param name="id">The transfer ID.</param>
        Task<AgentTransferDto?> GetByIdAsync(int id);

        /// <summary>Initiates a new agent transfer (REQ-65, REQ-66, REQ-68, REQ-69, REQ-71).</summary>
        /// <param name="request">Transfer creation data.</param>
        Task<AgentTransferDto> CreateAsync(CreateAgentTransferRequest request);

        /// <summary>Approves a permanent transfer (REQ-67).</summary>
        /// <param name="id">The transfer ID.</param>
        Task<bool> ApproveAsync(int id);

        /// <summary>Deletes a transfer by ID.</summary>
        /// <param name="id">The transfer ID.</param>
        Task<bool> DeleteAsync(int id);
    }
}