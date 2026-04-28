namespace PoliceManagementSystem.DTOs.AgentTransfers
{
    /// <summary>Payload for initiating an agent transfer (REQ-65, REQ-66, REQ-68, REQ-69, REQ-71).</summary>
    public class CreateAgentTransferRequest
    {
        public int AgentId { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}