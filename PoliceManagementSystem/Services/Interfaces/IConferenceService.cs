using PoliceManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services.Interfaces
{
    public interface IConferenceService
    {
        Task<IEnumerable<Conference>> GetAllAsync();
        Task<Conference?> GetByIdAsync(int id);
        Task<Conference> CreateAsync(Conference conference, IEnumerable<int>? participantIds);
        Task<bool> StartConferenceAsync(int id);
        Task<bool> AddParticipantsAsync(int conferenceId, IEnumerable<int> participantIds);
        Task<bool> DeleteAsync(int id);
    }
}