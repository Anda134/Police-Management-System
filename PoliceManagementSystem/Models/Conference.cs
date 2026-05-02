namespace PoliceManagementSystem.Models
{
    /// <summary>Represents a scheduled communication conference between agents (REQ-33 to REQ-48).</summary>
    public class Conference
    {
        public int Id { get; set; }

        /// <summary>Reason or topic for the conference (REQ-42).</summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>Personal callsign for the conference (REQ-43).</summary>
        public string Callsign { get; set; } = string.Empty;

        /// <summary>Priority level - higher value = higher priority (REQ-45, REQ-46).</summary>
        public int Priority { get; set; }

        /// <summary>Scheduled date and time for the conference (REQ-44).</summary>
        public DateTime ScheduledAt { get; set; }

        /// <summary>Whether the conference has started (REQ-48).</summary>
        public bool IsStarted { get; set; } = false;

        /// <summary>ID of the agent who organized the conference (REQ-45).</summary>
        public int OrganizerId { get; set; }

        /// <summary>Navigation property for the organizer agent.</summary>
        public Agent Organizer { get; set; } = null!;

        /// <summary>Agents participating in the conference (REQ-35, REQ-36).</summary>
        public ICollection<Agent> Participants { get; set; } = new List<Agent>();
    }
}