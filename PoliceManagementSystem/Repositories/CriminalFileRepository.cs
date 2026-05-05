using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Repositories.Interfaces;

namespace PoliceManagementSystem.Repositories
{
    public class CriminalFileRepository : Repository<CriminalFile>, ICriminalFileRepository
    {
        public CriminalFileRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CriminalFile>> GetFilesWithDetailsAsync()
        {
            return await _context.CriminalFiles
                .Include(file => file.PoliceStation)
                .Include(file => file.Agent)
                .ToListAsync();
        }

        public async Task<CriminalFile?> GetFileWithDetailsByIdAsync(int id)
        {
            return await _context.CriminalFiles
                .Include(file => file.PoliceStation)
                .Include(file => file.Agent)
                .FirstOrDefaultAsync(file => file.Id == id);
        }

        public async Task<IEnumerable<CriminalFile>> SearchAsync(
            string? title,
            string? category,
            int? agentId)
        {
            var query = _context.CriminalFiles
                .Include(file => file.PoliceStation)
                .Include(file => file.Agent)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(file => file.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(file => file.Category.Contains(category));
            }

            if (agentId.HasValue)
            {
                query = query.Where(file => file.AgentId == agentId.Value);
            }

            return await query.ToListAsync();
        }
    }
}