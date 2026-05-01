using PoliceManagementSystem.DTOs.PoliceStations;

namespace PoliceManagementSystem.Services.Interfaces
{
    /// <summary>Defines CRUD operations for police stations (REQ-1 to REQ-8).</summary>
    public interface IPoliceStationService
    {
        /// <summary>Returns all police stations.</summary>
        Task<IEnumerable<PoliceStationDto>> GetAllAsync();

        /// <summary>Returns a single police station by ID.</summary>
        /// <param name="id">The station ID.</param>
        Task<PoliceStationDto?> GetByIdAsync(int id);

        /// <summary>Creates a new police station.</summary>
        /// <param name="request">Station creation data.</param>
        Task<PoliceStationDto> CreateAsync(CreatePoliceStationRequest request);

        /// <summary>Updates an existing police station.</summary>
        /// <param name="id">The station ID.</param>
        /// <param name="request">Updated station data.</param>
        Task<bool> UpdateAsync(int id, UpdatePoliceStationRequest request);

        /// <summary>Deletes a police station by ID.</summary>
        /// <param name="id">The station ID.</param>
        Task<bool> DeleteAsync(int id);
    }
}