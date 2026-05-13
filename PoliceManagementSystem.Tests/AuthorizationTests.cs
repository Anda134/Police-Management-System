using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.CriminalFiles;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for authorization rules (REQ-55, REQ-75, REQ-76).</summary>
    public class AuthorizationTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<(PoliceStation station, Agent agent)> SeedDataAsync(AppDbContext context)
        {
            var station = new PoliceStation { Name = "Sectia 1", Address = "Addr", Latitude = 44.1, Longitude = 23.1 };
            context.PoliceStations.Add(station);
            await context.SaveChangesAsync();

            var agent = new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A1", Role = "Agent", PoliceStationId = station.Id };
            context.Agents.Add(agent);
            await context.SaveChangesAsync();

            return (station, agent);
        }

        /// <summary>REQ-55: Only Admin role should be able to delete criminal files.</summary>
        [Fact]
        public async Task DeleteFile_AdminRole_Succeeds()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedDataAsync(context);

            var file = new CriminalFile
            {
                Title = "Test File",
                Category = "Theft",
                Status = "Open",
                PoliceStationId = station.Id,
                AgentId = agent.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.CriminalFiles.Add(file);
            await context.SaveChangesAsync();

            var service = new CriminalFileService(context, auditService);

            // Act
            var result = await service.DeleteAsync(file.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(0, context.CriminalFiles.Count());
        }

        /// <summary>REQ-55: Deleting non-existent file should return false.</summary>
        [Fact]
        public async Task DeleteFile_NonExistentFile_ReturnsFalse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var service = new CriminalFileService(context, auditService);

            // Act
            var result = await service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        /// <summary>REQ-76: Admin user should have Admin role.</summary>
        [Fact]
        public async Task RegisterUser_AdminRole_HasAdminRole()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "SuperSecretKeyForTestingPurposesOnly123!" },
                    { "Jwt:Issuer", "PoliceManagementSystem" },
                    { "Jwt:Audience", "PoliceManagementSystemUsers" },
                    { "Jwt:ExpiryMinutes", "60" }
                })
                .Build();
            var authService = new AuthenticationService(context, config, auditService);

            // Act
            var user = await authService.RegisterAsync(new DTOs.Auth.RegisterRequest
            {
                Username = "newadmin",
                Email = "newadmin@police.ro",
                Password = "Admin123!",
                Role = "Admin"
            });

            // Assert
            Assert.NotNull(user);
            Assert.Equal(UserRole.Admin, user.Role);
        }

        /// <summary>REQ-75: ChiefInspector role should be parsed correctly.</summary>
        [Fact]
        public async Task RegisterUser_ChiefInspectorRole_HasCorrectRole()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "SuperSecretKeyForTestingPurposesOnly123!" },
                    { "Jwt:Issuer", "PoliceManagementSystem" },
                    { "Jwt:Audience", "PoliceManagementSystemUsers" },
                    { "Jwt:ExpiryMinutes", "60" }
                })
                .Build();
            var authService = new AuthenticationService(context, config, auditService);

            // Act
            var user = await authService.RegisterAsync(new DTOs.Auth.RegisterRequest
            {
                Username = "chiefinspector",
                Email = "ci@police.ro",
                Password = "Password123!",
                Role = "ChiefInspector"
            });

            // Assert
            Assert.NotNull(user);
            Assert.Equal(UserRole.ChiefInspector, user.Role);
        }

        /// <summary>REQ-75: StationHead role should be parsed correctly.</summary>
        [Fact]
        public async Task RegisterUser_StationHeadRole_HasCorrectRole()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "SuperSecretKeyForTestingPurposesOnly123!" },
                    { "Jwt:Issuer", "PoliceManagementSystem" },
                    { "Jwt:Audience", "PoliceManagementSystemUsers" },
                    { "Jwt:ExpiryMinutes", "60" }
                })
                .Build();
            var authService = new AuthenticationService(context, config, auditService);

            // Act
            var user = await authService.RegisterAsync(new DTOs.Auth.RegisterRequest
            {
                Username = "stationhead",
                Email = "sh@police.ro",
                Password = "Password123!",
                Role = "StationHead"
            });

            // Assert
            Assert.NotNull(user);
            Assert.Equal(UserRole.StationHead, user.Role);
        }

        /// <summary>REQ-73: Inactive user should not be able to login.</summary>
        [Fact]
        public async Task Login_InactiveUser_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "SuperSecretKeyForTestingPurposesOnly123!" },
                    { "Jwt:Issuer", "PoliceManagementSystem" },
                    { "Jwt:Audience", "PoliceManagementSystemUsers" },
                    { "Jwt:ExpiryMinutes", "60" }
                })
                .Build();

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

            var authService = new AuthenticationService(context, config, auditService);

            // Act
            var result = await authService.LoginAsync(new DTOs.Auth.LoginRequest
            {
                Username = "inactive",
                Password = "Password123!"
            }, "127.0.0.1");

            // Assert
            Assert.Null(result);
        }
    }
}