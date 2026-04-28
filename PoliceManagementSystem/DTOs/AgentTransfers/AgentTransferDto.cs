namespace PoliceManagementSystem.DTOs.AgentTransfers
{
    /// <summary>Read model returned to clients for an agent transfer.</summary>
    public class AgentTransferDto
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public int FromStationId { get; set; }
        public string FromStationName { get; set; } = string.Empty;
        public int ToStationId { get; set; }
        public string ToStationName { get; set; } = string.Empty;
        public bool IsPermanent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
}