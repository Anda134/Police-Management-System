namespace PoliceManagementSystem.Services.Interfaces
{
    /// <summary>Records audit events for legal traceability (REQ-79, REQ-80).</summary>
    public interface IAuditLoggingService
    {
        /// <summary>Logs an action to the audit trail.</summary>
        /// <param name="action">Action performed (e.g. LOGIN, FILE_UPDATE).</param>
        /// <param name="entityType">Type of entity affected (e.g. User, CriminalFile).</param>
        /// <param name="entityId">ID of the affected entity.</param>
        /// <param name="userId">ID of the user who performed the action.</param>
        /// <param name="oldValue">Previous value before change.</param>
        /// <param name="newValue">New value after change.</param>
        /// <param name="success">Whether the action succeeded.</param>
        /// <param name="ipAddress">IP address of the user.</param>
        Task LogAsync(string action, string entityType, string? entityId = null,
                      int? userId = null, string? oldValue = null, string? newValue = null,
                      bool success = true, string? ipAddress = null);
    }
}