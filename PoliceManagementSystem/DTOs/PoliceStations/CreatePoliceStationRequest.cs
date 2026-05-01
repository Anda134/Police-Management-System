namespace PoliceManagementSystem.DTOs.PoliceStations
{
    /// <summary>Payload for creating a new police station (REQ-1, REQ-2).</summary>
    public class CreatePoliceStationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}