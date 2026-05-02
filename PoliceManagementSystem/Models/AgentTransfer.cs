namespace PoliceManagementSystem.Models
{
    /// <summary>Represents a temporary or permanent transfer of an agent between stations (REQ-65 to REQ-72).</summary>
    public class AgentTransfer
    {
        public int Id { get; set; }

        /// <summary>ID of the agent being transferred (REQ-65, REQ-66).</summary>
        public int AgentId { get; set; }

        /// <summary>Navigation property for the transferred agent.</summary>
        public Agent Agent { get; set; } = null!;

        /// <summary>ID of the station the agent is transferred from (REQ-65).</summary>
        public int FromStationId { get; set; }

        /// <summary>Navigation property for the source station.</summary>
        public PoliceStation FromStation { get; set; } = null!;

        /// <summary>ID of the station the agent is transferred to (REQ-65).</summary>
        public int ToStationId { get; set; }

        /// <summary>Navigation property for the destination station.</summary>
        public PoliceStation ToStation { get; set; } = null!;

        /// <summary>Whether the transfer is permanent or temporary (REQ-65, REQ-66).</summary>
        public bool IsPermanent { get; set; }

        /// <summary>Start date of the transfer (REQ-68).</summary>
        public DateTime StartDate { get; set; }

        /// <summary>End date of a temporary transfer - system auto-reverts after this date (REQ-69, REQ-70).</summary>
        public DateTime? EndDate { get; set; }

        /// <summary>Reason provided for the transfer (REQ-71).</summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>Whether the transfer has been approved by ChiefInspector (REQ-67).</summary>
        public bool IsApproved { get; set; } = false;
    }
}