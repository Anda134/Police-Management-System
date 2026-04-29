namespace PoliceManagementSystem.Models
{
    /// <summary>Immutable snapshot of a criminal file at a point in time (REQ-56).</summary>
    public class CriminalFileHistory
    {
        public int Id { get; set; }
        public int CriminalFileId { get; set; }
        public CriminalFile CriminalFile { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int AgentId { get; set; }
        public int PoliceStationId { get; set; }
        public string ChangedByUsername { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}