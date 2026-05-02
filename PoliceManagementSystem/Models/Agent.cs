namespace PoliceManagementSystem.Models
{
    /// <summary>Represents a police agent assigned to a station (REQ-17 to REQ-24).</summary>
    public class Agent
    {
        public int Id { get; set; }

        /// <summary>Agent's first name.</summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>Agent's last name.</summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>Unique badge number identifying the agent (REQ-17).</summary>
        public string Badge { get; set; } = string.Empty;

        /// <summary>Role of the agent e.g. Agent, StationHead, ChiefInspector (REQ-19).</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>ID of the station this agent belongs to (REQ-17).</summary>
        public int PoliceStationId { get; set; }

        /// <summary>Navigation property for the agent's station.</summary>
        public PoliceStation PoliceStation { get; set; } = null!;

        /// <summary>ID of the superior agent in the hierarchy (REQ-21).</summary>
        public int? SuperiorId { get; set; }

        /// <summary>Navigation property for the superior agent (REQ-21).</summary>
        public Agent? Superior { get; set; }

        /// <summary>Agents that report to this agent (REQ-21, REQ-23).</summary>
        public ICollection<Agent> Subordinates { get; set; } = new List<Agent>();
    }
}