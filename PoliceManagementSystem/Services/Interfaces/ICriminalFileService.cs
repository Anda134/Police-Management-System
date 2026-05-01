using PoliceManagementSystem.DTOs.CriminalFiles;

namespace PoliceManagementSystem.Services.Interfaces
{
    /// <summary>Defines operations for managing criminal files (REQ-49 to REQ-64).</summary>
    public interface ICriminalFileService
    {
        /// <summary>Returns all criminal files.</summary>
        Task<IEnumerable<CriminalFileDto>> GetAllAsync();

        /// <summary>Returns a single criminal file by ID.</summary>
        /// <param name="id">The file ID.</param>
        Task<CriminalFileDto?> GetByIdAsync(int id);

        /// <summary>Searches files by title, category or agent (REQ-57, REQ-58, REQ-59).</summary>
        /// <param name="title">Optional title filter.</param>
        /// <param name="category">Optional category filter.</param>
        /// <param name="agentId">Optional agent ID filter.</param>
        Task<IEnumerable<CriminalFileDto>> SearchAsync(string? title, string? category, int? agentId);

        /// <summary>Creates a new criminal file (REQ-49).</summary>
        /// <param name="request">File creation data.</param>
        Task<CriminalFileDto> CreateAsync(CreateCriminalFileRequest request);

        /// <summary>Updates an existing criminal file (REQ-52, REQ-53).</summary>
        /// <param name="id">The file ID.</param>
        /// <param name="request">Updated file data.</param>
        Task<bool> UpdateAsync(int id, UpdateCriminalFileRequest request);

        /// <summary>Transfers a file to another station (REQ-60, REQ-62, REQ-63, REQ-64).</summary>
        /// <param name="fileId">The file ID.</param>
        /// <param name="newStationId">The destination station ID.</param>
        /// <param name="userId">The user performing the transfer.</param>
        Task<bool> TransferAsync(int fileId, int newStationId, int userId);

        /// <summary>Deletes a criminal file. Requires authorization (REQ-55).</summary>
        /// <param name="id">The file ID.</param>
        Task<bool> DeleteAsync(int id);

        /// <summary>Returns the version history for a criminal file (REQ-56).</summary>
        /// <param name="fileId">The file ID.</param>
        Task<IEnumerable<CriminalFileHistoryDto>> GetHistoryAsync(int fileId);

    }
}