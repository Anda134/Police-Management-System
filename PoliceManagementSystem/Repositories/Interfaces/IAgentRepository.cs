using PoliceManagementSystem.Models;

namespace PoliceManagementSystem.Repositories.Interfaces
{
    public interface IAgentRepository
        : IRepository<Agent>
    {
        Task<IEnumerable<Agent>>
            GetAgentsWithHierarchyAsync();
    }
}