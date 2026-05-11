namespace PoliceManagementSystem.DTOs.CriminalFiles
{
    /// <summary>Read model returned to clients for a criminal file.</summary>
    public class CriminalFileDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int PoliceStationId { get; set; }
        public string PoliceStationName { get; set; } = string.Empty;
        public int AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}