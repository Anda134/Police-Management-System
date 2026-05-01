namespace PoliceManagementSystem.DTOs.Conferences
{
    /// <summary>Payload for creating a new conference (REQ-41, REQ-42, REQ-43, REQ-44).</summary>
    public class CreateConferenceRequest
    {
        public string Reason { get; set; } = string.Empty;
        public string Callsign { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int OrganizerId { get; set; }
        public List<int> ParticipantIds { get; set; } = new();
    }
}