namespace PoliceManagementSystem.Models
{
    public class CriminalFile
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int PoliceStationId { get; set; }
        public PoliceStation PoliceStation { get; set; } = null!;
        public int AgentId { get; set; }
        public Agent Agent { get; set; } = null!;
    }
}