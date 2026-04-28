namespace PoliceManagementSystem.DTOs.CriminalFiles
{
    /// <summary>Payload for updating an existing criminal file (REQ-52, REQ-53).</summary>
    public class UpdateCriminalFileRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int AgentId { get; set; }
    }
}