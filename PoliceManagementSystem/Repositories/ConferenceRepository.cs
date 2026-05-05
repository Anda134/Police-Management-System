using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Repositories.Interfaces;

namespace PoliceManagementSystem.Repositories
{
    public class ConferenceRepository : Repository<Conference>, IConferenceRepository
    {
        public ConferenceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Conference>> GetConferencesWithDetailsAsync()
        {
            return await _context.Conferences
                .Include(conference => conference.Organizer)
                .Include(conference => conference.Participants)
                .ToListAsync();
        }

        public async Task<Conference?> GetConferenceWithDetailsByIdAsync(int id)
        {
            return await _context.Conferences
                .Include(conference => conference.Organizer)
                .Include(conference => conference.Participants)
                .FirstOrDefaultAsync(conference => conference.Id == id);
        }
    }
}