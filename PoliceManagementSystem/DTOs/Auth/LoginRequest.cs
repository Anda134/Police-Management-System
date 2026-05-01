namespace PoliceManagementSystem.DTOs.Auth
{
    /// <summary>Credentials submitted by the user at login.</summary>
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}