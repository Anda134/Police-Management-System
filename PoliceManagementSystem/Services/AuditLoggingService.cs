using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Services
{
    /// <summary>Persists audit entries to the database.</summary>
    public class AuditLoggingService : IAuditLoggingService
    {
        private readonly AppDbContext _context;

        /// <summary>Initializes a new instance of AuditLoggingService.</summary>
        /// <param name="context">The database context.</param>
        public AuditLoggingService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Creates and saves an audit log entry.</summary>
        public async Task LogAsync(string action, string entityType, string? entityId = null,
                                   int? userId = null, string? oldValue = null, string? newValue = null,
                                   bool success = true, string? ipAddress = null)
        {
            var entry = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                OldValue = oldValue,
                NewValue = newValue,
                Success = success,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(entry);
            await _context.SaveChangesAsync();
        }
    }
}