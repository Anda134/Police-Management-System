using PoliceManagementSystem.Models;

namespace PoliceManagementSystem.Repositories.Interfaces
{
    public interface IConferenceRepository : IRepository<Conference>
    {
        Task<IEnumerable<Conference>> GetConferencesWithDetailsAsync();

        Task<Conference?> GetConferenceWithDetailsByIdAsync(int id);
    }
}