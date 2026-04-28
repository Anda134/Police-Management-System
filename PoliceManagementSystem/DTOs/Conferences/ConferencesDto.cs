namespace PoliceManagementSystem.DTOs.Conferences
{
    /// <summary>Read model returned to clients for a conference.</summary>
    public class ConferenceDto
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Callsign { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime ScheduledAt { get; set; }
        public bool IsStarted { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public List<string> ParticipantNames { get; set; } = new();
    }
}