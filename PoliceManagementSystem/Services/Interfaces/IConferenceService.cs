using PoliceManagementSystem.DTOs.Conferences;

namespace PoliceManagementSystem.Services.Interfaces
{
    /// <summary>Defines operations for managing conferences (REQ-33 to REQ-48).</summary>
    public interface IConferenceService
    {
        /// <summary>Returns all conferences.</summary>
        Task<IEnumerable<ConferenceDto>> GetAllAsync();

        /// <summary>Returns a single conference by ID.</summary>
        /// <param name="id">The conference ID.</param>
        Task<ConferenceDto?> GetByIdAsync(int id);

        /// <summary>Creates a new conference request (REQ-41, REQ-42, REQ-43, REQ-44).</summary>
        /// <param name="request">Conference creation data.</param>
        Task<ConferenceDto> CreateAsync(CreateConferenceRequest request);

        /// <summary>Starts a scheduled conference (REQ-48).</summary>
        /// <param name="id">The conference ID.</param>
        Task<bool> StartConferenceAsync(int id);

        /// <summary>Adds participants to a conference (REQ-35).</summary>
        /// <param name="conferenceId">The conference ID.</param>
        /// <param name="participantIds">List of agent IDs to add.</param>
        Task<bool> AddParticipantsAsync(int conferenceId, IEnumerable<int> participantIds);

        /// <summary>Deletes a conference by ID.</summary>
        /// <param name="id">The conference ID.</param>
        Task<bool> DeleteAsync(int id);
    }
}