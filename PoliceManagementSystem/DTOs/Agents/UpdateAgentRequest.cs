namespace PoliceManagementSystem.DTOs.Agents
{
    /// <summary>Payload for updating an existing agent (REQ-19).</summary>
    public class UpdateAgentRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Badge { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int PoliceStationId { get; set; }
        public int? SuperiorId { get; set; }
    }
}