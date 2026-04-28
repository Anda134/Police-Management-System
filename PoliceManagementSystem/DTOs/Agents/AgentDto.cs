namespace PoliceManagementSystem.DTOs.Agents
{
    /// <summary>Read model returned to clients for a police agent.</summary>
    public class AgentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Badge { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int PoliceStationId { get; set; }
        public string PoliceStationName { get; set; } = string.Empty;
        public int? SuperiorId { get; set; }
        public string? SuperiorName { get; set; }
    }
}