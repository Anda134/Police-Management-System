using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.AgentTransfers;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for AgentTransferService (REQ-65 to REQ-72).</summary>
    public class AgentTransferServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<(PoliceStation station1, PoliceStation station2, Agent agent)> SeedDataAsync(AppDbContext context)
        {
            var station1 = new PoliceStation { Name = "Sectia 1", Address = "Addr 1", Latitude = 44.1, Longitude = 23.1 };
            var station2 = new PoliceStation { Name = "Sectia 2", Address = "Addr 2", Latitude = 44.2, Longitude = 23.2 };
            context.PoliceStations.AddRange(station1, station2);
            await context.SaveChangesAsync();

            var agent = new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A1", Role = "Agent", PoliceStationId = station1.Id };
            context.Agents.Add(agent);
            await context.SaveChangesAsync();

            return (station1, station2, agent);
        }

        /// <summary>REQ-65: Creating a temporary transfer with valid data should succeed.</summary>
        [Fact]
        public async Task CreateAsync_TemporaryTransfer_ReturnsTransfer()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station1, station2, agent) = await SeedDataAsync(context);
            var service = new AgentTransferService(context);
            var request = new CreateAgentTransferRequest
            {
                AgentId = agent.Id,
                FromStationId = station1.Id,
                ToStationId = station2.Id,
                IsPermanent = false,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Reason = "Investigatie speciala"
            };

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(agent.Id, result.AgentId);
            Assert.False(result.IsPermanent);
            Assert.False(result.IsApproved);
        }

        /// <summary>REQ-66: Creating a permanent transfer should succeed.</summary>
        [Fact]
        public async Task CreateAsync_PermanentTransfer_ReturnsTransfer()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station1, station2, agent) = await SeedDataAsync(context);
            var service = new AgentTransferService(context);
            var request = new CreateAgentTransferRequest
            {
                AgentId = agent.Id,
                FromStationId = station1.Id,
                ToStationId = station2.Id,
                IsPermanent = true,
                StartDate = DateTime.UtcNow,
                Reason = "Transfer permanent"
            };

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsPermanent);
            Assert.False(result.IsApproved);
        }

        /// <summary>REQ-65: Transfer with same from/to station should throw exception.</summary>
        [Fact]
        public async Task CreateAsync_SameStations_ThrowsArgumentException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station1, station2, agent) = await SeedDataAsync(context);
            var service = new AgentTransferService(context);
            var request = new CreateAgentTransferRequest
            {
                AgentId = agent.Id,
                FromStationId = station1.Id,
                ToStationId = station1.Id,
                IsPermanent = false,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Reason = "Test"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        }

        /// <summary>REQ-65: Temporary transfer without end date should throw exception.</summary>
        [Fact]
        public async Task CreateAsync_TemporaryWithoutEndDate_ThrowsArgumentException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station1, station2, agent) = await SeedDataAsync(context);
            var service = new AgentTransferService(context);
            var request = new CreateAgentTransferRequest
            {
                AgentId = agent.Id,
                FromStationId = station1.Id,
                ToStationId = station2.Id,
                IsPermanent = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                Reason = "Test"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        }

        /// <summary>REQ-67: ApproveAsync should approve transfer and update agent station for permanent transfers.</summary>
        [Fact]
        public async Task ApproveAsync_PermanentTransfer_UpdatesAgentStation()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station1, station2, agent) = await SeedDataAsync(context);
            var transfer = new AgentTransfer
            {
                AgentId = agent.Id,
                FromStationId = station1.Id,
                ToStationId = station2.Id,
                IsPermanent = true,
                StartDate = DateTime.UtcNow,
                Reason = "Transfer permanent",
                IsApproved = false
            };
            context.AgentTransfers.Add(transfer);
            await context.SaveChangesAsync();
            var service = new AgentTransferService(context);

            // Act
            var result = await service.ApproveAsync(transfer.Id);

            // Assert
            Assert.True(result);
            var updatedAgent = await context.Agents.FindAsync(agent.Id);
            Assert.Equal(station2.Id, updatedAgent!.PoliceStationId);
        }

        /// <summary>REQ-72: GetAllAsync should return all transfers.</summary>
        [Fact]
        public async Task GetAllAsync_ReturnsAllTransfers()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station1, station2, agent) = await SeedDataAsync(context);
            context.AgentTransfers.AddRange(
                new AgentTransfer { AgentId = agent.Id, FromStationId = station1.Id, ToStationId = station2.Id, IsPermanent = false, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), Reason = "Test 1", IsApproved = false },
                new AgentTransfer { AgentId = agent.Id, FromStationId = station2.Id, ToStationId = station1.Id, IsPermanent = true, StartDate = DateTime.UtcNow, Reason = "Test 2", IsApproved = false }
            );
            await context.SaveChangesAsync();
            var service = new AgentTransferService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        /// <summary>DeleteAsync should remove transfer from database.</summary>
        [Fact]
        public async Task DeleteAsync_ExistingTransfer_ReturnsTrue()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var (station1, station2, agent) = await SeedDataAsync(context);
            var transfer = new AgentTransfer
            {
                AgentId = agent.Id,
                FromStationId = station1.Id,
                ToStationId = station2.Id,
                IsPermanent = false,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Reason = "Test",
                IsApproved = false
            };
            context.AgentTransfers.Add(transfer);
            await context.SaveChangesAsync();
            var service = new AgentTransferService(context);

            // Act
            var result = await service.DeleteAsync(transfer.Id);

            // Assert
            Assert.True(result);
            Assert.Equal(0, context.AgentTransfers.Count());
        }
    }
}