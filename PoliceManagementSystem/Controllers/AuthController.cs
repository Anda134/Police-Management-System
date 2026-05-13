using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PoliceManagementSystem.DTOs.Auth;
using PoliceManagementSystem.Services.Interfaces;

namespace PoliceManagementSystem.Controllers
{
    /// <summary>Handles authentication and user account management (REQ-73 to REQ-80).</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        /// <summary>Initializes a new instance of AuthController.</summary>
        /// <param name="authService">The authentication service.</param>
        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        /// <summary>Authenticates a user and returns a JWT token.</summary>
        /// <param name="request">Login credentials.</param>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Invalid login request.");

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var response = await _authService.LoginAsync(request, ipAddress);

            if (response == null)
                return Unauthorized("Invalid credentials or account inactive.");

            return Ok(response);
        }

        /// <summary>Registers a new user account. Admin only (REQ-76).</summary>
        /// <param name="request">Registration data.</param>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Invalid registration request.");

            var user = await _authService.RegisterAsync(request);

            if (user == null)
                return Conflict("Username already exists.");

            return Ok(new { user.Id, user.Username, user.Role });
        }

        /// <summary>Returns the current authenticated user's info.</summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return Ok(new { username, role });
        }

        /// <summary>Returns all users. Admin only (REQ-76).</summary>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>Deletes a user account. Admin only (REQ-76).</summary>
        /// <param name="id">The user ID to delete.</param>
        [HttpDelete("users/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _authService.DeleteUserAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}