using PoliceManagementSystem.Models;

namespace PoliceManagementSystem.Repositories.Interfaces
{
    public interface IAgentTransferRepository : IRepository<AgentTransfer>
    {
        Task<IEnumerable<AgentTransfer>> GetTransfersWithDetailsAsync();

        Task<AgentTransfer?> GetTransferWithDetailsByIdAsync(int id);
    }
}