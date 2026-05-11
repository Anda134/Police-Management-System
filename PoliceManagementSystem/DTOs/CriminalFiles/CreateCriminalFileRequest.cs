namespace PoliceManagementSystem.DTOs.CriminalFiles
{
    /// <summary>Payload for creating a new criminal file (REQ-49, REQ-50).</summary>
    public class CreateCriminalFileRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int PoliceStationId { get; set; }
        public int AgentId { get; set; }
        public string? Details { get; set; }
    }
}