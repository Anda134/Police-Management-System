using PoliceManagementSystem.Models;

namespace PoliceManagementSystem.Repositories.Interfaces
{
    public interface IPoliceStationRepository
        : IRepository<PoliceStation>
    {
        Task<IEnumerable<PoliceStation>>
            GetStationsWithAgentsAsync();
    }
}