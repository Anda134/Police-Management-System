using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.Conferences;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Services
{
    /// <summary>Handles operations for conferences (REQ-33 to REQ-48).</summary>
    public class ConferenceService : IConferenceService
    {
        private readonly AppDbContext _context;

        /// <summary>Initializes a new instance of ConferenceService.</summary>
        /// <param name="context">The database context.</param>
        public ConferenceService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Returns all conferences with organizer and participant info.</summary>
        public async Task<IEnumerable<ConferenceDto>> GetAllAsync()
        {
            return await _context.Conferences
                .Include(c => c.Organizer)
                .Include(c => c.Participants)
                .Select(c => new ConferenceDto
                {
                    Id = c.Id,
                    Reason = c.Reason,
                    Callsign = c.Callsign,
                    Priority = c.Priority,
                    ScheduledAt = c.ScheduledAt,
                    IsStarted = c.IsStarted,
                    OrganizerId = c.OrganizerId,
                    OrganizerName = c.Organizer.FirstName + " " + c.Organizer.LastName,
                    ParticipantNames = c.Participants
                        .Select(p => p.FirstName + " " + p.LastName)
                        .ToList()
                })
                .ToListAsync();
        }

        /// <summary>Returns a single conference by ID.</summary>
        /// <param name="id">The conference ID.</param>
        public async Task<ConferenceDto?> GetByIdAsync(int id)
        {
            var conference = await _context.Conferences
                .Include(c => c.Organizer)
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conference is null) return null;

            return new ConferenceDto
            {
                Id = conference.Id,
                Reason = conference.Reason,
                Callsign = conference.Callsign,
                Priority = conference.Priority,
                ScheduledAt = conference.ScheduledAt,
                IsStarted = conference.IsStarted,
                OrganizerId = conference.OrganizerId,
                OrganizerName = conference.Organizer.FirstName + " " + conference.Organizer.LastName,
                ParticipantNames = conference.Participants
                    .Select(p => p.FirstName + " " + p.LastName)
                    .ToList()
            };
        }

        /// <summary>Creates a new conference (REQ-41 to REQ-46).</summary>
        /// <param name="request">Conference creation data.</param>
        public async Task<ConferenceDto> CreateAsync(CreateConferenceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new ArgumentException("Reason is required.");

            if (string.IsNullOrWhiteSpace(request.Callsign))
                throw new ArgumentException("Callsign is required.");

            var organizerExists = await _context.Agents
                .AnyAsync(a => a.Id == request.OrganizerId);
            if (!organizerExists)
                throw new ArgumentException("Organizer agent not found.");

            var conference = new Conference
            {
                Reason = request.Reason,
                Callsign = request.Callsign,
                Priority = request.Priority,
                ScheduledAt = request.ScheduledAt,
                OrganizerId = request.OrganizerId,
                IsStarted = false
            };

            if (request.ParticipantIds.Count > 0)
            {
                var participants = await _context.Agents
                    .Where(a => request.ParticipantIds.Contains(a.Id))
                    .ToListAsync();
                conference.Participants = participants;
            }

            _context.Conferences.Add(conference);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(conference.Id))!;
        }

        /// <summary>Starts a scheduled conference (REQ-48).</summary>
        /// <param name="id">The conference ID.</param>
        public async Task<bool> StartConferenceAsync(int id)
        {
            var conference = await _context.Conferences.FindAsync(id);
            if (conference is null) return false;

            conference.IsStarted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Adds participants to a conference (REQ-35).</summary>
        /// <param name="conferenceId">The conference ID.</param>
        /// <param name="participantIds">List of agent IDs to add.</param>
        public async Task<bool> AddParticipantsAsync(int conferenceId, IEnumerable<int> participantIds)
        {
            var conference = await _context.Conferences
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == conferenceId);

            if (conference is null) return false;

            var newParticipants = await _context.Agents
                .Where(a => participantIds.Contains(a.Id))
                .ToListAsync();

            foreach (var participant in newParticipants)
            {
                if (!conference.Participants.Any(p => p.Id == participant.Id))
                    conference.Participants.Add(participant);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Deletes a conference by ID.</summary>
        /// <param name="id">The conference ID.</param>
        public async Task<bool> DeleteAsync(int id)
        {
            var conference = await _context.Conferences.FindAsync(id);
            if (conference is null) return false;

            _context.Conferences.Remove(conference);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}