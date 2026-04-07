namespace PoliceManagementSystem.Models
{
    public class AgentTransfer
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public Agent Agent { get; set; } = null!;
        public int FromStationId { get; set; }
        public PoliceStation FromStation { get; set; } = null!;
        public int ToStationId { get; set; }
        public PoliceStation ToStation { get; set; } = null!;
        public bool IsPermanent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
    }
}