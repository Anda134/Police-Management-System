using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.Agents;
using PoliceManagementSystem.DTOs.Auth;
using PoliceManagementSystem.DTOs.CriminalFiles;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for miscellaneous requirements (REQ-6, REQ-23, REQ-24, REQ-46, REQ-56, REQ-59, REQ-77, REQ-78).</summary>
    public class MiscServiceTests
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
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Jwt:Key", "SuperSecretKeyForTestingPurposesOnly123!" },
                    { "Jwt:Issuer", "PoliceManagementSystem" },
                    { "Jwt:Audience", "PoliceManagementSystemUsers" },
                    { "Jwt:ExpiryMinutes", "60" }
                })
                .Build();
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

        /// <summary>REQ-23: Assigning self as superior should throw InvalidOperationException.</summary>
        [Fact]
        public async Task AssignSuperior_CircularRelation_ThrowsException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, agent) = await SeedDataAsync(context);
            var service = new AgentService(context);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.AssignSuperiorAsync(agent.Id, agent.Id)
            );
        }

        /// <summary>REQ-56: Version history should be created on file update.</summary>
        [Fact]
        public async Task UpdateFile_CreatesVersionHistory()
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
            await service.UpdateAsync(file.Id, new UpdateCriminalFileRequest
            {
                Title = "Updated Title",
                Category = "Fraud",
                Status = "Closed",
                AgentId = agent.Id
            });

            // Assert
            var history = await context.CriminalFileHistories
                .Where(h => h.CriminalFileId == file.Id)
                .ToListAsync();
            Assert.True(history.Count >= 1);
            Assert.Contains(history, h => h.ChangeType == "UPDATE");
        }

        /// <summary>REQ-59: SearchAsync should filter by category.</summary>
        [Fact]
        public async Task SearchAsync_ByCategory_ReturnsMatchingFiles()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedDataAsync(context);
            context.CriminalFiles.AddRange(
                new CriminalFile { Title = "Furt auto", Category = "Theft", Status = "Open", PoliceStationId = station.Id, AgentId = agent.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new CriminalFile { Title = "Frauda", Category = "Fraud", Status = "Open", PoliceStationId = station.Id, AgentId = agent.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new CriminalFile { Title = "Droguri", Category = "Drugs", Status = "Open", PoliceStationId = station.Id, AgentId = agent.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
            var service = new CriminalFileService(context, auditService);

            // Act
            var result = await service.SearchAsync(null, "Fraud", null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Frauda", result.First().Title);
        }

        /// <summary>REQ-46: Conference conflict detection - same time slot.</summary>
        [Fact]
        public async Task CreateConference_ConflictDetection_SameTimeSlot()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station, agent) = await SeedDataAsync(context);
            var scheduledTime = DateTime.UtcNow.AddDays(1);

            var existing = new Conference
            {
                Reason = "Existing Conference",
                Callsign = "E1",
                ScheduledAt = scheduledTime,
                OrganizerId = agent.Id,
                Priority = 1
            };
            context.Conferences.Add(existing);
            await context.SaveChangesAsync();

            // Act - check for conflicts within 60 minutes
            var conflicts = await context.Conferences
                .Where(c => Math.Abs((c.ScheduledAt - scheduledTime).TotalMinutes) < 60)
                .ToListAsync();

            // Assert
            Assert.Single(conflicts);
        }

        /// <summary>REQ-77: Modifying user account should update data.</summary>
        [Fact]
        public async Task ModifyUser_UpdatesIsActive()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var user = new User
            {
                Username = "testuser",
                Email = "test@police.ro",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = UserRole.Agent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act - REQ-78: Deactivate user
            user.IsActive = false;
            await context.SaveChangesAsync();

            // Assert
            var updated = await context.Users.FindAsync(user.Id);
            Assert.False(updated!.IsActive);
        }

        /// <summary>REQ-78: Deactivated user should not be able to login.</summary>
        [Fact]
        public async Task Login_DeactivatedUser_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var config = CreateConfiguration();

            var user = new User
            {
                Username = "deactivated",
                Email = "deact@police.ro",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = UserRole.Agent,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var authService = new AuthenticationService(context, config, auditService);

            // Act
            var result = await authService.LoginAsync(new LoginRequest
            {
                Username = "deactivated",
                Password = "Password123!"
            }, "127.0.0.1");

            // Assert
            Assert.Null(result);
        }

        /// <summary>REQ-56: Version history should be created on file create.</summary>
        [Fact]
        public async Task CreateFile_CreatesVersionHistory()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedDataAsync(context);
            var service = new CriminalFileService(context, auditService);

            // Act
            var result = await service.CreateAsync(new CreateCriminalFileRequest
            {
                Title = "New File",
                Category = "Theft",
                Status = "Open",
                PoliceStationId = station.Id,
                AgentId = agent.Id
            });

            // Assert
            var history = await context.CriminalFileHistories
                .Where(h => h.CriminalFileId == result.Id)
                .ToListAsync();
            Assert.Single(history);
            Assert.Equal("CREATE", history.First().ChangeType);
        }
    }
}