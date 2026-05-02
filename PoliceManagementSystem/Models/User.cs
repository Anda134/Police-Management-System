namespace PoliceManagementSystem.Models
{
    /// <summary>User account linked to a police agent or admin (REQ-73 to REQ-80).</summary>
    public class User
    {
        public int Id { get; set; }

        /// <summary>Unique username for login (REQ-73).</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Email address of the user.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Hashed password - never stored in plain text (REQ-73).</summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>Role determining access level in the system (REQ-74, REQ-75).</summary>
        public UserRole Role { get; set; }

        /// <summary>ID of the linked police agent, if any.</summary>
        public int? AgentId { get; set; }

        /// <summary>Navigation property for the linked agent.</summary>
        public Agent? Agent { get; set; }

        /// <summary>Whether the account is active - inactive users cannot login (REQ-73).</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Date and time of the last successful login (REQ-79).</summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>Date and time the account was created (REQ-76).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>Defines the available user roles in the system (REQ-74).</summary>
    public enum UserRole
    {
        /// <summary>Full system access - can manage users and all data.</summary>
        Admin = 0,

        /// <summary>Can manage stations, agents, conferences and approve transfers.</summary>
        ChiefInspector = 1,

        /// <summary>Can manage agents and files within their station.</summary>
        StationHead = 2,

        /// <summary>Can view and update assigned criminal files.</summary>
        Agent = 3
    }
}