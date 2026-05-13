using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.Agents;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for AgentService (REQ-17 to REQ-24).</summary>
    public class AgentServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<PoliceStation> SeedStationAsync(AppDbContext context)
        {
            var station = new PoliceStation { Name = "Sectia 1", Address = "Addr", Latitude = 44.1, Longitude = 23.1 };
            context.PoliceStations.Add(station);
            await context.SaveChangesAsync();
            return station;
        }

        /// <summary>REQ-17: Creating an agent with valid data should succeed.</summary>
        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsAgent()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var station = await SeedStationAsync(context);
            var service = new AgentService(context);
            var request = new CreateAgentRequest
            {
                FirstName = "Alexandru",
                LastName = "Popescu",
                Badge = "AG001",
                Role = "Agent",
                PoliceStationId = station.Id
            };

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alexandru", result.FirstName);
            Assert.Equal("AG001", result.Badge);
        }

        /// <summary>REQ-17: Creating an agent without first name should throw exception.</summary>
        [Fact]
        public async Task CreateAsync_EmptyFirstName_ThrowsArgumentException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var station = await SeedStationAsync(context);
            var service = new AgentService(context);
            var request = new CreateAgentRequest
            {
                FirstName = "",
                Badge = "AG001",
                Role = "Agent",
                PoliceStationId = station.Id
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        }

        /// <summary>REQ-17: Creating an agent with non-existent station should throw exception.</summary>
        [Fact]
        public async Task CreateAsync_NonExistentStation_ThrowsArgumentException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AgentService(context);
            var request = new CreateAgentRequest
            {
                FirstName = "Alexandru",
                Badge = "AG001",
                Role = "Agent",
                PoliceStationId = 999
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        }

        /// <summary>REQ-21: GetAllAsync should return all agents.</summary>
        [Fact]
        public async Task GetAllAsync_ReturnsAllAgents()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var station = await SeedStationAsync(context);
            context.Agents.AddRange(
                new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A1", Role = "Agent", PoliceStationId = station.Id },
                new Agent { FirstName = "Maria", LastName = "Ion", Badge = "A2", Role = "Agent", PoliceStationId = station.Id }
            );
            await context.SaveChangesAsync();
            var service = new AgentService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        /// <summary>REQ-19: UpdateAsync should update agent data.</summary>
        [Fact]
        public async Task UpdateAsync_ValidRequest_UpdatesAgent()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var station = await SeedStationAsync(context);
            var agent = new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A1", Role = "Agent", PoliceStationId = station.Id };
            context.Agents.Add(agent);
            await context.SaveChangesAsync();
            var service = new AgentService(context);

            // Act
            var result = await service.UpdateAsync(agent.Id, new UpdateAgentRequest
            {
                FirstName = "Alexandru",
                LastName = "Pop",
                Badge = "A1",
                Role = "Agent",
                PoliceStationId = station.Id
            });

            // Assert
            Assert.True(result);
            var updated = await context.Agents.FindAsync(agent.Id);
            Assert.Equal("Alexandru", updated!.FirstName);
        }

        /// <summary>REQ-23: AssignSuperiorAsync with self as superior should throw exception.</summary>
        [Fact]
        public async Task AssignSuperiorAsync_SelfAsSuperior_ThrowsInvalidOperationException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var station = await SeedStationAsync(context);
            var agent = new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A1", Role = "Agent", PoliceStationId = station.Id };
            context.Agents.Add(agent);
            await context.SaveChangesAsync();
            var service = new AgentService(context);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.AssignSuperiorAsync(agent.Id, agent.Id)
            );
        }

        /// <summary>REQ-18: DeleteAsync should remove agent, clear subordinates and delete associated user.</summary>
        [Fact]
        public async Task DeleteAsync_ExistingAgent_ReturnsTrue()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var station = await SeedStationAsync(context);

            var superior = new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A1", Role = "Agent", PoliceStationId = station.Id };
            context.Agents.Add(superior);
            await context.SaveChangesAsync();

            var subordinate = new Agent { FirstName = "Maria", LastName = "Ion", Badge = "A2", Role = "Agent", PoliceStationId = station.Id, SuperiorId = superior.Id };
            context.Agents.Add(subordinate);
            await context.SaveChangesAsync();

            var user = new User { Username = "ion.pop", Email = "ion@police.ro", PasswordHash = "hash", Role = UserRole.Agent, AgentId = superior.Id, IsActive = true, CreatedAt = DateTime.UtcNow };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new AgentService(context);

            // Act
            var result = await service.DeleteAsync(superior.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(1, context.Agents.Count());
            Assert.Null(context.Agents.First().SuperiorId);
            Assert.Equal(0, context.Users.Count());
        }

        /// <summary>GetByIdAsync with non-existent ID should return null.</summary>
        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AgentService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}