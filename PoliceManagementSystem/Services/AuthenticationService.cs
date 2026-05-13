using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.Auth;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PoliceManagementSystem.Services
{
    /// <summary>Implements login, registration and JWT token generation.</summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAuditLoggingService _auditLogging;

        /// <summary>Initializes a new instance of AuthenticationService.</summary>
        /// <param name="context">The database context.</param>
        /// <param name="configuration">App configuration for JWT settings.</param>
        /// <param name="auditLogging">Audit logging service.</param>
        public AuthenticationService(AppDbContext context, IConfiguration configuration,
                                     IAuditLoggingService auditLogging)
        {
            _context = context;
            _configuration = configuration;
            _auditLogging = auditLogging;
        }

        /// <summary>Validates credentials and returns a JWT token on success.</summary>
        /// <param name="request">Login credentials.</param>
        /// <param name="ipAddress">IP address of the requester.</param>
        public async Task<LoginResponse?> LoginAsync(LoginRequest request, string? ipAddress)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

            var success = user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            await _auditLogging.LogAsync(
                action: "LOGIN",
                entityType: "User",
                entityId: user?.Id.ToString(),
                userId: user?.Id,
                success: success,
                ipAddress: ipAddress
            );

            if (!success) return null;

            user!.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("Jwt:ExpiryMinutes", 15));

            return new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role.ToString(),
                ExpiresAt = expiresAt
            };
        }

        /// <summary>Creates a new user account.</summary>
        /// <param name="request">Registration data.</param>
        public async Task<User?> RegisterAsync(RegisterRequest request)
        {
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == request.Username);
            if (usernameExists) return null;

            if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
                role = UserRole.Agent;

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role,
                AgentId = request.AgentId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>Finds a user by username.</summary>
        /// <param name="username">The username to search for.</param>
        public async Task<User?> GetByUsernameAsync(string username)
            => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        /// <summary>Returns all users (REQ-76).</summary>
        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    Role = u.Role.ToString(),
                    u.IsActive,
                    u.LastLoginAt,
                    u.CreatedAt
                })
                .ToListAsync<object>();
        }

        /// <summary>Deletes a user by ID (REQ-76).</summary>
        /// <param name="id">The user ID to delete.</param>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Generates a signed JWT token for the given user.</summary>
        /// <param name="user">The authenticated user.</param>
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes", 15);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("agentId", user.AgentId?.ToString() ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}