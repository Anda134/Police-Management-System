using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services
{
    public class PoliceStationService : IPoliceStationService
    {
        private readonly AppDbContext _context;

        public PoliceStationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PoliceStation>> GetAllAsync()
        {
            return await _context.PoliceStations
                .Include(ps => ps.Agents)
                .ToListAsync();
        }

        public async Task<PoliceStation?> GetByIdAsync(int id)
        {
            return await _context.PoliceStations
                .Include(ps => ps.Agents)
                .FirstOrDefaultAsync(ps => ps.Id == id);
        }

        public async Task<PoliceStation> CreateAsync(PoliceStation station)
        {
            _context.PoliceStations.Add(station);
            await _context.SaveChangesAsync();
            return station;
        }

        public async Task<bool> UpdateAsync(int id, PoliceStation station)
        {
            var existing = await _context.PoliceStations.FindAsync(id);
            if (existing == null) return false;

            existing.Name = station.Name;
            existing.Address = station.Address;
            existing.Latitude = station.Latitude;
            existing.Longitude = station.Longitude;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.PoliceStations.FindAsync(id);
            if (existing == null) return false;

            _context.PoliceStations.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}