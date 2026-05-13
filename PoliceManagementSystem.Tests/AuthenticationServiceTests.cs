using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.Auth;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for AuthenticationService (REQ-73 to REQ-80).</summary>
    public class AuthenticationServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private IConfiguration CreateConfiguration()
        {
            var config = new Dictionary<string, string?>
            {
                { "Jwt:Key", "SuperSecretKeyForTestingPurposesOnly123!" },
                { "Jwt:Issuer", "PoliceManagementSystem" },
                { "Jwt:Audience", "PoliceManagementSystemUsers" },
                { "Jwt:ExpiryMinutes", "60" }
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
        }

        /// <summary>REQ-76: Registering a new user with valid data should succeed.</summary>
        [Fact]
        public async Task RegisterAsync_ValidRequest_ReturnsUser()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var config = CreateConfiguration();
            var auditService = new AuditLoggingService(context);
            var service = new AuthenticationService(context, config, auditService);
            var request = new RegisterRequest
            {
                Username = "ion.popescu",
                Email = "ion@police.ro",
                Password = "Password123!",
                Role = "Agent"
            };

            // Act
            var result = await service.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ion.popescu", result.Username);
            Assert.Equal(UserRole.Agent, result.Role);
        }

        /// <summary>REQ-76: Registering with duplicate username should return null.</summary>
        [Fact]
        public async Task RegisterAsync_DuplicateUsername_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var config = CreateConfiguration();
            var auditService = new AuditLoggingService(context);
            var service = new AuthenticationService(context, config, auditService);

            var existingUser = new User
            {
                Username = "ion.popescu",
                Email = "ion@police.ro",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = UserRole.Agent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var request = new RegisterRequest
            {
                Username = "ion.popescu",
                Email = "ion2@police.ro",
                Password = "Password123!",
                Role = "Agent"
            };

            // Act
            var result = await service.RegisterAsync(request);

            // Assert
            Assert.Null(result);
        }

        /// <summary>REQ-73: Login with valid credentials should return token.</summary>
        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var config = CreateConfiguration();
            var auditService = new AuditLoggingService(context);
            var service = new AuthenticationService(context, config, auditService);

            var user = new User
            {
                Username = "admin",
                Email = "admin@police.ro",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var request = new LoginRequest
            {
                Username = "admin",
                Password = "Admin123!"
            };

            // Act
            var result = await service.LoginAsync(request, "127.0.0.1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("admin", result.Username);
            Assert.Equal("Admin", result.Role);
            Assert.NotEmpty(result.Token);
        }

        /// <summary>REQ-73: Login with invalid password should return null.</summary>
        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var config = CreateConfiguration();
            var auditService = new AuditLoggingService(context);
            var service = new AuthenticationService(context, config, auditService);

            var user = new User
            {
                Username = "admin",
                Email = "admin@police.ro",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var request = new LoginRequest
            {
                Username = "admin",
                Password = "WrongPassword!"
            };

            // Act
            var result = await service.LoginAsync(request, "127.0.0.1");

            // Assert
            Assert.Null(result);
        }

        /// <summary>REQ-73: Login with inactive user should return null.</summary>
        [Fact]
        public async Task LoginAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var config = CreateConfiguration();
            var auditService = new AuditLoggingService(context);
            var service = new AuthenticationService(context, config, auditService);

            var user = new User
            {
                Username = "inactive",
                Email = "inactive@police.ro",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = UserRole.Agent,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var request = new LoginRequest
            {
                Username = "inactive",
                Password = "Password123!"
            };

            // Act
            var result = await service.LoginAsync(request, "127.0.0.1");

            // Assert
            Assert.Null(result);
        }

        /// <summary>REQ-76: GetAllUsersAsync should return all users.</summary>
        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var config = CreateConfiguration();
            var auditService = new AuditLoggingService(context);
            context.Users.AddRange(
                new User { Username = "user1", Email = "u1@police.ro", PasswordHash = "hash", Role = UserRole.Agent, IsActive = true, CreatedAt = DateTime.UtcNow },
                new User { Username = "user2", Email = "u2@police.ro", PasswordHash = "hash", Role = UserRole.Admin, IsActive = true, CreatedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
            var service = new AuthenticationService(context, config, auditService);

            // Act
            var result = await service.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        /// <summary>REQ-76: DeleteUserAsync should remove user from database.</summary>
        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var config = CreateConfiguration();
            var auditService = new AuditLoggingService(context);
            var user = new User
            {
                Username = "todelete",
                Email = "del@police.ro",
                PasswordHash = "hash",
                Role = UserRole.Agent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            var service = new AuthenticationService(context, config, auditService);

            // Act
            var result = await service.DeleteUserAsync(user.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(0, context.Users.Count());
        }

        /// <summary>DeleteUserAsync with non-existent ID should return false.</summary>
        [Fact]
        public async Task DeleteUserAsync_NonExistentId_ReturnsFalse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var config = CreateConfiguration();
            var auditService = new AuditLoggingService(context);
            var service = new AuthenticationService(context, config, auditService);

            // Act
            var result = await service.DeleteUserAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}