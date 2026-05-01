using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.PoliceStations;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Services
{
    /// <summary>Handles CRUD operations for police stations (REQ-1 to REQ-8).</summary>
    public class PoliceStationService : IPoliceStationService
    {
        private readonly AppDbContext _context;

        /// <summary>Initializes a new instance of PoliceStationService.</summary>
        /// <param name="context">The database context.</param>
        public PoliceStationService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Returns all police stations with agent count.</summary>
        public async Task<IEnumerable<PoliceStationDto>> GetAllAsync()
        {
            return await _context.PoliceStations
                .Include(ps => ps.Agents)
                .Select(ps => new PoliceStationDto
                {
                    Id = ps.Id,
                    Name = ps.Name,
                    Address = ps.Address,
                    Latitude = ps.Latitude,
                    Longitude = ps.Longitude,
                    AgentCount = ps.Agents.Count
                })
                .ToListAsync();
        }

        /// <summary>Returns a single police station by ID.</summary>
        /// <param name="id">The station ID.</param>
        public async Task<PoliceStationDto?> GetByIdAsync(int id)
        {
            var station = await _context.PoliceStations
                .Include(ps => ps.Agents)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (station is null) return null;

            return new PoliceStationDto
            {
                Id = station.Id,
                Name = station.Name,
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                AgentCount = station.Agents.Count
            };
        }

        /// <summary>Creates a new police station.</summary>
        /// <param name="request">Station creation data.</param>
        public async Task<PoliceStationDto> CreateAsync(CreatePoliceStationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Station name is required.");

            var station = new PoliceStation
            {
                Name = request.Name,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            _context.PoliceStations.Add(station);
            await _context.SaveChangesAsync();

            return new PoliceStationDto
            {
                Id = station.Id,
                Name = station.Name,
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                AgentCount = 0
            };
        }

        /// <summary>Updates an existing police station.</summary>
        /// <param name="id">The station ID.</param>
        /// <param name="request">Updated station data.</param>
        public async Task<bool> UpdateAsync(int id, UpdatePoliceStationRequest request)
        {
            var station = await _context.PoliceStations.FindAsync(id);
            if (station is null) return false;

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Station name is required.");

            station.Name = request.Name;
            station.Address = request.Address;
            station.Latitude = request.Latitude;
            station.Longitude = request.Longitude;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Deletes a police station by ID.</summary>
        /// <param name="id">The station ID.</param>
        public async Task<bool> DeleteAsync(int id)
        {
            var station = await _context.PoliceStations.FindAsync(id);
            if (station is null) return false;

            _context.PoliceStations.Remove(station);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}