namespace PoliceManagementSystem.DTOs.Auth
{
    /// <summary>JWT token and user info returned after successful login.</summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}