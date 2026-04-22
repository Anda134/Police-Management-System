using PoliceManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services.Interfaces
{
    public interface IAgentTransferService
    {
        Task<IEnumerable<AgentTransfer>> GetAllAsync();
        Task<AgentTransfer?> GetByIdAsync(int id);
        Task<AgentTransfer> CreateAsync(AgentTransfer transfer);
        Task<bool> ApproveAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}