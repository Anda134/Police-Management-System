using Microsoft.EntityFrameworkCore;
using PoliceManagementSystem.Data;
using PoliceManagementSystem.DTOs.CriminalFiles;
using PoliceManagementSystem.Models;
using PoliceManagementSystem.Services;
using Xunit;

namespace PoliceManagementSystem.Tests
{
    /// <summary>Unit tests for CriminalFileService (REQ-49 to REQ-64).</summary>
    public class CriminalFileServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<(PoliceStation station, Agent agent)> SeedBasicDataAsync(AppDbContext context)
        {
            var station = new PoliceStation { Name = "Sectia 1", Address = "Addr", Latitude = 44.1, Longitude = 23.1 };
            context.PoliceStations.Add(station);
            await context.SaveChangesAsync();

            var agent = new Agent { FirstName = "Ion", LastName = "Pop", Badge = "A1", Role = "Agent", PoliceStationId = station.Id };
            context.Agents.Add(agent);
            await context.SaveChangesAsync();

            return (station, agent);
        }

        /// <summary>REQ-49: Creating a file with valid data should succeed.</summary>
        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsFile()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedBasicDataAsync(context);
            var service = new CriminalFileService(context, auditService);
            var request = new CreateCriminalFileRequest
            {
                Title = "Furt auto",
                Category = "Theft",
                Status = "Open",
                PoliceStationId = station.Id,
                AgentId = agent.Id
            };

            // Act
            var result = await service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Furt auto", result.Title);
            Assert.Equal("Theft", result.Category);
        }

        /// <summary>REQ-49: Creating a file without title should throw exception.</summary>
        [Fact]
        public async Task CreateAsync_EmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedBasicDataAsync(context);
            var service = new CriminalFileService(context, auditService);
            var request = new CreateCriminalFileRequest
            {
                Title = "",
                Category = "Theft",
                Status = "Open",
                PoliceStationId = station.Id,
                AgentId = agent.Id
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request));
        }

        /// <summary>REQ-51: GetByIdAsync should return correct file.</summary>
        [Fact]
        public async Task GetByIdAsync_ExistingFile_ReturnsFile()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedBasicDataAsync(context);
            var file = new CriminalFile
            {
                Title = "Frauda",
                Category = "Fraud",
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
            var result = await service.GetByIdAsync(file.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Frauda", result.Title);
        }

        /// <summary>REQ-51: GetByIdAsync with non-existent ID should return null.</summary>
        [Fact]
        public async Task GetByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var service = new CriminalFileService(context, auditService);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        /// <summary>REQ-52, REQ-53: UpdateAsync should update file data.</summary>
        [Fact]
        public async Task UpdateAsync_ValidRequest_UpdatesFile()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedBasicDataAsync(context);
            var file = new CriminalFile
            {
                Title = "Old Title",
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
            var result = await service.UpdateAsync(file.Id, new UpdateCriminalFileRequest
            {
                Title = "New Title",
                Category = "Fraud",
                Status = "Closed",
                AgentId = agent.Id
            });

            // Assert
            Assert.True(result);
            var updated = await context.CriminalFiles.FindAsync(file.Id);
            Assert.Equal("New Title", updated!.Title);
            Assert.Equal("Closed", updated.Status);
        }

        /// <summary>REQ-55: DeleteAsync should remove file from database.</summary>
        [Fact]
        public async Task DeleteAsync_ExistingFile_ReturnsTrue()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedBasicDataAsync(context);
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

        /// <summary>REQ-57, REQ-58, REQ-59: SearchAsync should filter by title.</summary>
        [Fact]
        public async Task SearchAsync_ByTitle_ReturnsMatchingFiles()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station, agent) = await SeedBasicDataAsync(context);
            context.CriminalFiles.AddRange(
                new CriminalFile { Title = "Furt auto", Category = "Theft", Status = "Open", PoliceStationId = station.Id, AgentId = agent.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new CriminalFile { Title = "Frauda", Category = "Fraud", Status = "Open", PoliceStationId = station.Id, AgentId = agent.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
            var service = new CriminalFileService(context, auditService);

            // Act
            var result = await service.SearchAsync("Furt", null, null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Furt auto", result.First().Title);
        }

        /// <summary>REQ-60, REQ-64: TransferAsync should update file station.</summary>
        [Fact]
        public async Task TransferAsync_ValidStations_UpdatesFileStation()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var auditService = new AuditLoggingService(context);
            var (station1, agent) = await SeedBasicDataAsync(context);
            var station2 = new PoliceStation { Name = "Sectia 2", Address = "Addr 2", Latitude = 44.2, Longitude = 23.2 };
            context.PoliceStations.Add(station2);
            await context.SaveChangesAsync();
            var file = new CriminalFile
            {
                Title = "Test",
                Category = "Theft",
                Status = "Open",
                PoliceStationId = station1.Id,
                AgentId = agent.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.CriminalFiles.Add(file);
            await context.SaveChangesAsync();
            var service = new CriminalFileService(context, auditService);

            // Act
            var result = await service.TransferAsync(file.Id, station2.Id, 1);

            // Assert
            Assert.True(result);
            var updated = await context.CriminalFiles.FindAsync(file.Id);
            Assert.Equal(station2.Id, updated!.PoliceStationId);
        }
    }
}