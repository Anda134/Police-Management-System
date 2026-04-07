namespace PoliceManagementSystem.Models
{
    public class Conference
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Callsign { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime ScheduledAt { get; set; }
        public bool IsStarted { get; set; } = false;
        public int OrganizerId { get; set; }
        public Agent Organizer { get; set; } = null!;
        public ICollection<Agent> Participants { get; set; } = new List<Agent>();
    }
}