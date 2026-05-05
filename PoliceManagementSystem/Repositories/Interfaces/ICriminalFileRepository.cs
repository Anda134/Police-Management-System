using PoliceManagementSystem.Models;

namespace PoliceManagementSystem.Repositories.Interfaces
{
    public interface ICriminalFileRepository : IRepository<CriminalFile>
    {
        Task<IEnumerable<CriminalFile>> GetFilesWithDetailsAsync();

        Task<CriminalFile?> GetFileWithDetailsByIdAsync(int id);

        Task<IEnumerable<CriminalFile>> SearchAsync(
            string? title,
            string? category,
            int? agentId);
    }
}