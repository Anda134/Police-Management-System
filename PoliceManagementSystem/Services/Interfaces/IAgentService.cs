using PoliceManagementSystem.DTOs.Agents;

namespace PoliceManagementSystem.Services.Interfaces
{
    /// <summary>Defines operations for managing police agents (REQ-17 to REQ-24).</summary>
    public interface IAgentService
    {
        /// <summary>Returns all agents.</summary>
        Task<IEnumerable<AgentDto>> GetAllAsync();

        /// <summary>Returns a single agent by ID.</summary>
        /// <param name="id">The agent ID.</param>
        Task<AgentDto?> GetByIdAsync(int id);

        /// <summary>Creates a new agent.</summary>
        /// <param name="request">Agent creation data.</param>
        Task<AgentDto> CreateAsync(CreateAgentRequest request);

        /// <summary>Updates an existing agent.</summary>
        /// <param name="id">The agent ID.</param>
        /// <param name="request">Updated agent data.</param>
        Task<bool> UpdateAsync(int id, UpdateAgentRequest request);

        /// <summary>Deletes an agent by ID.</summary>
        /// <param name="id">The agent ID.</param>
        Task<bool> DeleteAsync(int id);

        /// <summary>Assigns or removes a superior for an agent (REQ-21, REQ-23).</summary>
        /// <param name="agentId">The agent ID.</param>
        /// <param name="superiorId">The superior's ID, or null to remove.</param>
        Task<bool> AssignSuperiorAsync(int agentId, int? superiorId);
    }
}