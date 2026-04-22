using PoliceManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services.Interfaces
{
    public interface IAgentService
    {
        Task<IEnumerable<Agent>> GetAllAsync();
        Task<Agent?> GetByIdAsync(int id);
        Task<Agent> CreateAsync(Agent agent);
        Task<bool> UpdateAsync(int id, Agent agent);
        Task<bool> DeleteAsync(int id);
        Task<bool> AssignSuperiorAsync(int agentId, int? superiorId);
    }
}