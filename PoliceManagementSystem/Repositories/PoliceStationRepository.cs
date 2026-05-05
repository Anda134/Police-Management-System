using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Repositories.Interfaces;

namespace PoliceManagementSystem.Repositories
{
    public class PoliceStationRepository
        : Repository<PoliceStation>,
          IPoliceStationRepository
    {
        public PoliceStationRepository(
            AppDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<PoliceStation>>
            GetStationsWithAgentsAsync()
        {
            return await _context.PoliceStations
                .Include(ps => ps.Agents)
                .ToListAsync();
        }
    }
}