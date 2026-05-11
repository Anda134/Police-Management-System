namespace PoliceManagementSystem.Models
{
    /// <summary>Represents a criminal activity file managed by a station (REQ-49 to REQ-59).</summary>
    public class CriminalFile
    {
        public int Id { get; set; }

        /// <summary>Title or name of the criminal file (REQ-49).</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Category of the criminal activity e.g. Theft, Fraud (REQ-50, REQ-59).</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Current status of the file e.g. Open, Closed, Pending (REQ-53, REQ-54).</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Date and time the file was created (REQ-49).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Date and time the file was last updated (REQ-52, REQ-56).</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>ID of the station responsible for this file (REQ-49).</summary>
        public int PoliceStationId { get; set; }

        /// <summary>Navigation property for the responsible station.</summary>
        public PoliceStation PoliceStation { get; set; } = null!;

        /// <summary>ID of the agent assigned to this file (REQ-49).</summary>
        public int AgentId { get; set; }

        /// <summary>Navigation property for the assigned agent.</summary>
        public Agent Agent { get; set; } = null!;
        /// <summary>Detailed investigation notes, suspects, witnesses and interrogations.</summary>
        public string? Details { get; set; }
    }
}