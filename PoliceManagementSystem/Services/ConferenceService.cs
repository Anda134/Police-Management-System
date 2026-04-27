using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services
{
    public class ConferenceService : IConferenceService
    {
        private readonly AppDbContext _context;

        public ConferenceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Conference>> GetAllAsync()
        {
            return await _context.Conferences
                .Include(c => c.Organizer)
                .Include(c => c.Participants)
                .ToListAsync();
        }

        public async Task<Conference?> GetByIdAsync(int id)
        {
            return await _context.Conferences
                .Include(c => c.Organizer)
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Conference> CreateAsync(Conference conference, IEnumerable<int>? participantIds)
        {
            if (participantIds != null)
            {
                var participants = await _context.Agents
                    .Where(a => participantIds.Contains(a.Id))
                    .ToListAsync();

                conference.Participants = participants;
            }

            _context.Conferences.Add(conference);
            await _context.SaveChangesAsync();
            return conference;
        }

        public async Task<bool> StartConferenceAsync(int id)
        {
            var conference = await _context.Conferences.FindAsync(id);
            if (conference == null) return false;

            conference.IsStarted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddParticipantsAsync(int conferenceId, IEnumerable<int> participantIds)
        {
            var conference = await _context.Conferences
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == conferenceId);

            if (conference == null) return false;

            var participants = await _context.Agents
                .Where(a => participantIds.Contains(a.Id))
                .ToListAsync();

            foreach (var participant in participants)
            {
                if (!conference.Participants.Any(p => p.Id == participant.Id))
                {
                    conference.Participants.Add(participant);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var conference = await _context.Conferences.FindAsync(id);
            if (conference == null) return false;

            _context.Conferences.Remove(conference);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}