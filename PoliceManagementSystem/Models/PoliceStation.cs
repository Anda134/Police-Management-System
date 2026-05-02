namespace PoliceManagementSystem.Models
{
    /// <summary>Represents a police station with its location and assigned agents (REQ-1 to REQ-16).</summary>
    public class PoliceStation
    {
        public int Id { get; set; }

        /// <summary>Name of the police station (REQ-8).</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Street address of the police station (REQ-8).</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Geographical latitude coordinate of the station (REQ-4, REQ-7).</summary>
        public double Latitude { get; set; }

        /// <summary>Geographical longitude coordinate of the station (REQ-4, REQ-7).</summary>
        public double Longitude { get; set; }

        /// <summary>Agents assigned to this station (REQ-17).</summary>
        public ICollection<Agent> Agents { get; set; } = new List<Agent>();
    }
}
