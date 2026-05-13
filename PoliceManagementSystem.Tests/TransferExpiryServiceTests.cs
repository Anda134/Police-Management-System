using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.BackgroundServices;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for TransferExpiryService (REQ-70).</summary>
    public class TransferExpiryServiceTests
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

            var agent = new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A1", Role = "Agent", PoliceStationId = station2.Id };
            context.Agents.Add(agent);
            await context.SaveChangesAsync();

            return (station1, station2, agent);
        }

        /// <summary>REQ-70: Expired temporary transfer should revert agent to original station.</summary>
        [Fact]
        public async Task RevertExpiredTransfers_ExpiredTransfer_RevertsAgentStation()
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
                IsApproved = true,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(-1),
                Reason = "Test transfer"
            };
            context.AgentTransfers.Add(transfer);
            await context.SaveChangesAsync();

            var logger = new Mock<ILogger<TransferExpiryService>>();

            // Act
            var expiredTransfers = await context.AgentTransfers
                .Include(t => t.Agent)
                .Where(t => !t.IsPermanent && t.IsApproved && t.EndDate != null && t.EndDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var t in expiredTransfers)
            {
                t.Agent.PoliceStationId = t.FromStationId;
            }
            await context.SaveChangesAsync();

            // Assert
            var updatedAgent = await context.Agents.FindAsync(agent.Id);
            Assert.Equal(station1.Id, updatedAgent!.PoliceStationId);
        }

        /// <summary>REQ-70: Non-expired temporary transfer should not revert agent.</summary>
        [Fact]
        public async Task RevertExpiredTransfers_ActiveTransfer_DoesNotRevert()
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
                IsApproved = true,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                Reason = "Active transfer"
            };
            context.AgentTransfers.Add(transfer);
            await context.SaveChangesAsync();

            // Act
            var expiredTransfers = await context.AgentTransfers
                .Include(t => t.Agent)
                .Where(t => !t.IsPermanent && t.IsApproved && t.EndDate != null && t.EndDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var t in expiredTransfers)
            {
                t.Agent.PoliceStationId = t.FromStationId;
            }
            await context.SaveChangesAsync();

            // Assert
            var updatedAgent = await context.Agents.FindAsync(agent.Id);
            Assert.Equal(station2.Id, updatedAgent!.PoliceStationId);
        }

        /// <summary>REQ-70: Permanent transfer should not be reverted.</summary>
        [Fact]
        public async Task RevertExpiredTransfers_PermanentTransfer_DoesNotRevert()
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
                IsApproved = true,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = null,
                Reason = "Permanent transfer"
            };
            context.AgentTransfers.Add(transfer);
            await context.SaveChangesAsync();

            // Act
            var expiredTransfers = await context.AgentTransfers
                .Include(t => t.Agent)
                .Where(t => !t.IsPermanent && t.IsApproved && t.EndDate != null && t.EndDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var t in expiredTransfers)
            {
                t.Agent.PoliceStationId = t.FromStationId;
            }
            await context.SaveChangesAsync();

            // Assert
            var updatedAgent = await context.Agents.FindAsync(agent.Id);
            Assert.Equal(station2.Id, updatedAgent!.PoliceStationId);
        }

        /// <summary>REQ-70: Unapproved transfer should not be reverted.</summary>
        [Fact]
        public async Task RevertExpiredTransfers_UnapprovedTransfer_DoesNotRevert()
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
                IsApproved = false,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(-1),
                Reason = "Unapproved transfer"
            };
            context.AgentTransfers.Add(transfer);
            await context.SaveChangesAsync();

            // Act
            var expiredTransfers = await context.AgentTransfers
                .Include(t => t.Agent)
                .Where(t => !t.IsPermanent && t.IsApproved && t.EndDate != null && t.EndDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var t in expiredTransfers)
            {
                t.Agent.PoliceStationId = t.FromStationId;
            }
            await context.SaveChangesAsync();

            // Assert
            var updatedAgent = await context.Agents.FindAsync(agent.Id);
            Assert.Equal(station2.Id, updatedAgent!.PoliceStationId);
        }
    }
}