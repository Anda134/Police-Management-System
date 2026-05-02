namespace PoliceManagementSystem.Models
{
    /// <summary>Immutable snapshot of a criminal file at a point in time (REQ-56).</summary>
    public class CriminalFileHistory
    {
        public int Id { get; set; }

        /// <summary>ID of the criminal file this snapshot belongs to (REQ-56).</summary>
        public int CriminalFileId { get; set; }

        /// <summary>Navigation property for the parent criminal file.</summary>
        public CriminalFile CriminalFile { get; set; } = null!;

        /// <summary>Title of the file at the time of the snapshot.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Category of the file at the time of the snapshot (REQ-50).</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Status of the file at the time of the snapshot (REQ-53, REQ-54).</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>ID of the agent assigned at the time of the snapshot.</summary>
        public int AgentId { get; set; }

        /// <summary>ID of the station responsible at the time of the snapshot.</summary>
        public int PoliceStationId { get; set; }

        /// <summary>Username of the user who made the change (REQ-79).</summary>
        public string ChangedByUsername { get; set; } = string.Empty;

        /// <summary>Type of change performed e.g. Created, Updated, StatusChanged.</summary>
        public string ChangeType { get; set; } = string.Empty;

        /// <summary>Date and time when the change was made (REQ-56).</summary>
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}