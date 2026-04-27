using PoliceManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services.Interfaces
{
    public interface ICriminalFileService
    {
        Task<IEnumerable<CriminalFile>> GetAllAsync();
        Task<CriminalFile?> GetByIdAsync(int id);
        Task<IEnumerable<CriminalFile>> SearchAsync(string? title, string? category, int? agentId);
        Task<CriminalFile> CreateAsync(CriminalFile file);
        Task<bool> UpdateAsync(int id, CriminalFile file);
        Task<bool> TransferAsync(int fileId, int newStationId);
        Task<bool> DeleteAsync(int id);
    }
}