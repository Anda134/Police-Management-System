namespace PoliceManagementSystem.DTOs.PoliceStations
{
    /// <summary>Read model returned to clients for a police station.</summary>
    public class PoliceStationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int AgentCount { get; set; }
    }
}