namespace PoliceManagementSystem.Models
{
    /// <summary>Immutable audit trail for all significant actions (REQ-79, REQ-80).</summary>
    public class AuditLog
    {
        public int Id { get; set; }

        /// <summary>ID of the user who performed the action (REQ-79).</summary>
        public int? UserId { get; set; }

        /// <summary>Navigation property for the user who performed the action.</summary>
        public User? User { get; set; }

        /// <summary>Action performed e.g. LOGIN, FILE_UPDATE, TRANSFER_REVERTED (REQ-79, REQ-80).</summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>Type of entity affected e.g. User, CriminalFile, AgentTransfer.</summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>ID of the affected entity.</summary>
        public string? EntityId { get; set; }

        /// <summary>Previous value before the change (REQ-56).</summary>
        public string? OldValue { get; set; }

        /// <summary>New value after the change (REQ-56).</summary>
        public string? NewValue { get; set; }

        /// <summary>Whether the action was successful (REQ-80).</summary>
        public bool Success { get; set; } = true;

        /// <summary>IP address of the user who performed the action (REQ-79).</summary>
        public string? IpAddress { get; set; }

        /// <summary>Date and time the action was performed.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}