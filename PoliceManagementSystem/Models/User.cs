namespace PoliceManagementSystem.Models
{
    /// <summary>User account linked to a police agent or admin.</summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int? AgentId { get; set; }
        public Agent? Agent { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum UserRole
    {
        Admin = 0,
        ChiefInspector = 1,
        StationHead = 2,
        Agent = 3
    }
}