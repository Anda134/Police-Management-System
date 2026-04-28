namespace PoliceManagementSystem.DTOs.Auth
{
    /// <summary>Data required to create a new user account.</summary>
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Agent";
        public int? AgentId { get; set; }
    }
}