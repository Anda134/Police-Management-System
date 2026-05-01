using PoliceManagementSystem.DTOs.Auth;
using PoliceManagementSystem.Models;

namespace PoliceManagementSystem.Services.Interfaces
{
    /// <summary>Handles login, registration and token generation.</summary>
    public interface IAuthenticationService
    {
        /// <summary>Validates credentials and returns a JWT token on success.</summary>
        /// <param name="request">Login credentials.</param>
        /// <param name="ipAddress">IP address of the requester.</param>
        Task<LoginResponse?> LoginAsync(LoginRequest request, string? ipAddress);

        /// <summary>Creates a new user account.</summary>
        /// <param name="request">Registration data.</param>
        Task<User?> RegisterAsync(RegisterRequest request);

        /// <summary>Finds a user by username.</summary>
        /// <param name="username">The username to search for.</param>
        Task<User?> GetByUsernameAsync(string username);
    }
}