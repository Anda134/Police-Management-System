using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceManagementSystem.Services
{
    public class CriminalFileService : ICriminalFileService
    {
        private readonly AppDbContext _context;

        public CriminalFileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CriminalFile>> GetAllAsync()
        {
            return await _context.CriminalFiles
                .Include(cf => cf.PoliceStation)
                .Include(cf => cf.Agent)
                .ToListAsync();
        }

        public async Task<CriminalFile?> GetByIdAsync(int id)
        {
            return await _context.CriminalFiles
                .Include(cf => cf.PoliceStation)
                .Include(cf => cf.Agent)
                .FirstOrDefaultAsync(cf => cf.Id == id);
        }

        public async Task<IEnumerable<CriminalFile>> SearchAsync(string? title, string? category, int? agentId)
        {
            var query = _context.CriminalFiles
                .Include(cf => cf.PoliceStation)
                .Include(cf => cf.Agent)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(f => f.Title.Contains(title));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(f => f.Category.Contains(category));

            if (agentId.HasValue)
                query = query.Where(f => f.AgentId == agentId.Value);

            return await query.ToListAsync();
        }

        public async Task<CriminalFile> CreateAsync(CriminalFile file)
        {
            file.CreatedAt = DateTime.UtcNow;
            file.UpdatedAt = DateTime.UtcNow;

            _context.CriminalFiles.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<bool> UpdateAsync(int id, CriminalFile file)
        {
            var existing = await _context.CriminalFiles.FindAsync(id);
            if (existing == null) return false;

            existing.Title = file.Title;
            existing.Category = file.Category;
            existing.Status = file.Status;
            existing.AgentId = file.AgentId;
            existing.PoliceStationId = file.PoliceStationId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransferAsync(int fileId, int newStationId)
        {
            var file = await _context.CriminalFiles.FindAsync(fileId);
            if (file == null) return false;

            file.PoliceStationId = newStationId;
            file.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.CriminalFiles.FindAsync(id);
            if (existing == null) return false;

            _context.CriminalFiles.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}