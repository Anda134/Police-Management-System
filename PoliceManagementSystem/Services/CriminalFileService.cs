using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.CriminalFiles;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Services
{
    /// <summary>Handles operations for criminal files (REQ-49 to REQ-64).</summary>
    public class CriminalFileService : ICriminalFileService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLoggingService _auditLogging;

        /// <summary>Initializes a new instance of CriminalFileService.</summary>
        /// <param name="context">The database context.</param>
        /// <param name="auditLogging">The audit logging service.</param>
        public CriminalFileService(AppDbContext context, IAuditLoggingService auditLogging)
        {
            _context = context;
            _auditLogging = auditLogging;
        }

        /// <summary>Returns all criminal files with station and agent info.</summary>
        public async Task<IEnumerable<CriminalFileDto>> GetAllAsync()
        {
            return await _context.CriminalFiles
                .Include(cf => cf.PoliceStation)
                .Include(cf => cf.Agent)
                .Select(cf => new CriminalFileDto
                {
                    Id = cf.Id,
                    Title = cf.Title,
                    Category = cf.Category,
                    Status = cf.Status,
                    CreatedAt = cf.CreatedAt,
                    UpdatedAt = cf.UpdatedAt,
                    PoliceStationId = cf.PoliceStationId,
                    PoliceStationName = cf.PoliceStation.Name,
                    AgentId = cf.AgentId,
                    AgentName = cf.Agent.FirstName + " " + cf.Agent.LastName
                })
                .ToListAsync();
        }

        /// <summary>Returns a single criminal file by ID.</summary>
        /// <param name="id">The file ID.</param>
        public async Task<CriminalFileDto?> GetByIdAsync(int id)
        {
            var file = await _context.CriminalFiles
                .Include(cf => cf.PoliceStation)
                .Include(cf => cf.Agent)
                .FirstOrDefaultAsync(cf => cf.Id == id);

            if (file is null) return null;

            return new CriminalFileDto
            {
                Id = file.Id,
                Title = file.Title,
                Category = file.Category,
                Status = file.Status,
                CreatedAt = file.CreatedAt,
                UpdatedAt = file.UpdatedAt,
                PoliceStationId = file.PoliceStationId,
                PoliceStationName = file.PoliceStation.Name,
                AgentId = file.AgentId,
                AgentName = file.Agent.FirstName + " " + file.Agent.LastName
            };
        }

        /// <summary>Searches files by title, category or agent (REQ-57, REQ-58, REQ-59).</summary>
        /// <param name="title">Optional title filter.</param>
        /// <param name="category">Optional category filter.</param>
        /// <param name="agentId">Optional agent ID filter.</param>
        public async Task<IEnumerable<CriminalFileDto>> SearchAsync(string? title, string? category, int? agentId)
        {
            var query = _context.CriminalFiles
                .Include(cf => cf.PoliceStation)
                .Include(cf => cf.Agent)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(cf => cf.Title.Contains(title));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(cf => cf.Category == category);

            if (agentId.HasValue)
                query = query.Where(cf => cf.AgentId == agentId.Value);

            return await query
                .Select(cf => new CriminalFileDto
                {
                    Id = cf.Id,
                    Title = cf.Title,
                    Category = cf.Category,
                    Status = cf.Status,
                    CreatedAt = cf.CreatedAt,
                    UpdatedAt = cf.UpdatedAt,
                    PoliceStationId = cf.PoliceStationId,
                    PoliceStationName = cf.PoliceStation.Name,
                    AgentId = cf.AgentId,
                    AgentName = cf.Agent.FirstName + " " + cf.Agent.LastName
                })
                .ToListAsync();
        }

        /// <summary>Creates a new criminal file (REQ-49, REQ-50).</summary>
        /// <param name="request">File creation data.</param>
        public async Task<CriminalFileDto> CreateAsync(CreateCriminalFileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required.");

            var stationExists = await _context.PoliceStations
                .AnyAsync(ps => ps.Id == request.PoliceStationId);
            if (!stationExists)
                throw new ArgumentException("Police station not found.");

            var agentExists = await _context.Agents
                .AnyAsync(a => a.Id == request.AgentId);
            if (!agentExists)
                throw new ArgumentException("Agent not found.");

            var file = new CriminalFile
            {
                Title = request.Title,
                Category = request.Category,
                Status = request.Status,
                PoliceStationId = request.PoliceStationId,
                AgentId = request.AgentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CriminalFiles.Add(file);
            await _context.SaveChangesAsync();

            await SaveHistoryAsync(file, "system", "CREATE");

            await _auditLogging.LogAsync(
                action: "FILE_CREATE",
                entityType: "CriminalFile",
                entityId: file.Id.ToString()
            );

            return (await GetByIdAsync(file.Id))!;
        }

        /// <summary>Updates an existing criminal file (REQ-52, REQ-53).</summary>
        /// <param name="id">The file ID.</param>
        /// <param name="request">Updated file data.</param>
        public async Task<bool> UpdateAsync(int id, UpdateCriminalFileRequest request)
        {
            var file = await _context.CriminalFiles.FindAsync(id);
            if (file is null) return false;

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required.");

            var oldValue = $"Title={file.Title},Status={file.Status}";

            file.Title = request.Title;
            file.Category = request.Category;
            file.Status = request.Status;
            file.AgentId = request.AgentId;
            file.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await SaveHistoryAsync(file, "system", "UPDATE");

            await _auditLogging.LogAsync(
                action: "FILE_UPDATE",
                entityType: "CriminalFile",
                entityId: id.ToString(),
                oldValue: oldValue,
                newValue: $"Title={file.Title},Status={file.Status}"
            );

            return true;
        }

        /// <summary>Transfers a file to another station (REQ-60, REQ-62, REQ-63, REQ-64).</summary>
        /// <param name="fileId">The file ID.</param>
        /// <param name="newStationId">The destination station ID.</param>
        /// <param name="userId">The user performing the transfer.</param>
        public async Task<bool> TransferAsync(int fileId, int newStationId, int userId)
        {
            var file = await _context.CriminalFiles.FindAsync(fileId);
            if (file is null) return false;

            var stationExists = await _context.PoliceStations
                .AnyAsync(ps => ps.Id == newStationId);
            if (!stationExists)
                throw new ArgumentException("Destination station not found.");

            var oldStationId = file.PoliceStationId;
            file.PoliceStationId = newStationId;
            file.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await SaveHistoryAsync(file, "system", "TRANSFER");

            await _auditLogging.LogAsync(
                action: "FILE_TRANSFER",
                entityType: "CriminalFile",
                entityId: fileId.ToString(),
                userId: userId,
                oldValue: $"StationId={oldStationId}",
                newValue: $"StationId={newStationId}"
            );

            return true;
        }

        /// <summary>Deletes a criminal file (REQ-55).</summary>
        /// <param name="id">The file ID.</param>
        public async Task<bool> DeleteAsync(int id)
        {
            var file = await _context.CriminalFiles.FindAsync(id);
            if (file is null) return false;

            _context.CriminalFiles.Remove(file);
            await _context.SaveChangesAsync();

            await _auditLogging.LogAsync(
                action: "FILE_DELETE",
                entityType: "CriminalFile",
                entityId: id.ToString()
            );

            return true;
        }

        /// <summary>Returns the version history for a criminal file (REQ-56).</summary>
        /// <param name="fileId">The file ID.</param>
        public async Task<IEnumerable<CriminalFileHistoryDto>> GetHistoryAsync(int fileId)
        {
            return await _context.CriminalFileHistories
                .Where(cfh => cfh.CriminalFileId == fileId)
                .OrderByDescending(cfh => cfh.ChangedAt)
                .Select(cfh => new CriminalFileHistoryDto
                {
                    Id = cfh.Id,
                    CriminalFileId = cfh.CriminalFileId,
                    Title = cfh.Title,
                    Category = cfh.Category,
                    Status = cfh.Status,
                    AgentId = cfh.AgentId,
                    PoliceStationId = cfh.PoliceStationId,
                    ChangedByUsername = cfh.ChangedByUsername,
                    ChangeType = cfh.ChangeType,
                    ChangedAt = cfh.ChangedAt
                })
                .ToListAsync();
        }

        /// <summary>Saves a snapshot of the file to history.</summary>
        /// <param name="file">The file to snapshot.</param>
        /// <param name="changedByUsername">Username of the user making the change.</param>
        /// <param name="changeType">Type of change (CREATE, UPDATE, TRANSFER).</param>
        private async Task SaveHistoryAsync(CriminalFile file, string changedByUsername, string changeType)
        {
            var history = new CriminalFileHistory
            {
                CriminalFileId = file.Id,
                Title = file.Title,
                Category = file.Category,
                Status = file.Status,
                AgentId = file.AgentId,
                PoliceStationId = file.PoliceStationId,
                ChangedByUsername = changedByUsername,
                ChangeType = changeType,
                ChangedAt = DateTime.UtcNow
            };

            _context.CriminalFileHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}