namespace PoliceManagementSystem.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Badge { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int PoliceStationId { get; set; }
        public PoliceStation PoliceStation { get; set; } = null!;
        public int? SuperiorId { get; set; }
        public Agent? Superior { get; set; }
        public ICollection<Agent> Subordinates { get; set; } = new List<Agent>();
        
    }
}