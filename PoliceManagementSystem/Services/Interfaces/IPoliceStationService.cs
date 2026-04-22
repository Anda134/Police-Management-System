using PoliceManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services.Interfaces
{
    public interface IPoliceStationService
    {
        Task<IEnumerable<PoliceStation>> GetAllAsync();
        Task<PoliceStation?> GetByIdAsync(int id);
        Task<PoliceStation> CreateAsync(PoliceStation station);
        Task<bool> UpdateAsync(int id, PoliceStation station);
        Task<bool> DeleteAsync(int id);
    }
}