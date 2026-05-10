namespace PoliceManagementSystem.DTOs.Agents
{
    /// <summary>Payload for creating a new police agent (REQ-17, REQ-18).</summary>
    public class CreateAgentRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Badge { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int PoliceStationId { get; set; }
        public int? SuperiorId { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? RoomAssignment { get; set; }
    }
}