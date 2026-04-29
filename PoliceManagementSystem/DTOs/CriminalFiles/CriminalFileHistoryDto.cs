namespace PoliceManagementSystem.DTOs.CriminalFiles
{
    /// <summary>Read model returned to clients for a criminal file history entry.</summary>
    public class CriminalFileHistoryDto
    {
        public int Id { get; set; }
        public int CriminalFileId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int AgentId { get; set; }
        public int PoliceStationId { get; set; }
        public string ChangedByUsername { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}