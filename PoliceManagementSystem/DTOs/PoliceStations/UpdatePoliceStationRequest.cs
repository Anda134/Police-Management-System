namespace PoliceManagementSystem.DTOs.PoliceStations
{
    /// <summary>Payload for updating an existing police station (REQ-3).</summary>
    public class UpdatePoliceStationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}